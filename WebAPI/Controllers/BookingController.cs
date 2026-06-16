using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Common; // Đảm bảo bạn đã tạo class Utils trong này hoặc sửa namespace cho đúng
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly IPaymentService _paymentService; // Thêm Service xử lý thanh toán
        private readonly AppDbContext _context;

        public BookingController(AppDbContext context, IBookingService bookingService, IPaymentService paymentService)
        {
            _context = context;
            _bookingService = bookingService;
            _paymentService = paymentService; // Khởi tạo Service thanh toán
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                  ?? User.FindFirst("id")?.Value;

                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized(new { message = "Không tìm thấy thông tin người dùng." });

                Guid userId = Guid.Parse(userIdClaim);

                // Gọi Service xử lý logic lưu DB chính xác đã được tính toán tiền an toàn
                var booking = await _bookingService.CreateAsync(userId, request);

                var resultDto = new
                {
                    Id = booking.Id,
                    HotelId = booking.HotelId,
                    CheckIn = booking.CheckIn,
                    CheckOut = booking.CheckOut,
                    TotalAmount = booking.TotalAmount,
                    Status = booking.Status.ToString(),
                    CreatedAt = booking.CreatedAt,
                    Rooms = booking.BookingDetails.Select(bd => new {
                        RoomId = bd.RoomId,
                        Nights = bd.Nights,
                        PricePerNight = bd.PricePerNight
                    }).ToList()
                };

                return Ok(resultDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("detail/{id}")]
        public async Task<IActionResult> GetBookingDetail(Guid id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Hotel)
                .Include(b => b.BookingDetails)
                    .ThenInclude(d => d.Room)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return NotFound(new { message = "Không tìm thấy thông tin đơn đặt phòng." });
            }

            var detailLine = booking.BookingDetails?.FirstOrDefault();
            var detail = new
            {
                Id = booking.Id,
                HotelName = booking.Hotel?.Name ?? "Khách sạn hệ thống",
                RoomName = detailLine?.Room?.Note ?? "Phòng tiêu chuẩn",
                PricePerNight = detailLine?.PricePerNight ?? 0,
                Nights = detailLine?.Nights ?? 0,
                CheckIn = booking.CheckIn,
                CheckOut = booking.CheckOut,
                TotalAmount = booking.TotalAmount,
                Status = booking.Status,
                CreatedAt = booking.CreatedAt
            };

            return Ok(detail);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookingDetailForPayment(Guid id)
        {
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return NotFound(new { message = "❌ Không tìm thấy đơn đặt phòng hợp lệ." });
            }

            // Sửa lại: Lấy trực tiếp TotalAmount từ DB, không tính toán lại ở đây
            var response = new
            {
                Id = booking.Id,
                HotelId = booking.HotelId,
                CheckIn = booking.CheckIn,
                CheckOut = booking.CheckOut,
                TotalAmount = booking.TotalAmount,
                Status = booking.Status.ToString()
            };

            return Ok(response);
        }

        /// <summary>
        /// API Xử lý yêu cầu thanh toán (Sinh link VNPAY trực tuyến hoặc chốt thu ngân tại quầy)
        /// </summary>
        [HttpPost("process-payment")]
        public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequest request)
        {
            try
            {
                // Lấy địa chỉ IP của Client gửi yêu cầu thanh toán để truyền sang VNPAY chống gian lận
                string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";

                // Gọi PaymentService để thực hiện logic sinh URL VNPAY hoặc thanh toán tiền mặt trực tiếp
                string result = await _paymentService.CreatePaymentUrlAsync(request.BookingId, request.PaymentMethod, ipAddress);

                if (result == "COUNTER_SUCCESS")
                {
                    return Ok(new { message = "Đặt phòng và thanh toán trực tiếp tại quầy thành công!", paymentMethod = "COUNTER" });
                }

                // Trả về chuỗi URL dẫn sang trang thanh toán VNPAY Sandbox cho Client
                return Ok(new { paymentUrl = result, paymentMethod = "VNPAY" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// API Webhook (IPN URL) - Cổng VNPAY sẽ tự động gọi ngầm tới endpoint này để trả kết quả thanh toán thực tế
        /// </summary>
        [HttpGet("vnpay-ipn")]
        public async Task<IActionResult> VnpayIpn()
        {
            string vnp_HashSecret = "YOUR_HASH_SECRET_HERE"; // Thay chuỗi Secret Key VNPAY của bạn vào đây
            var queries = Request.Query;

            // 1. Xác thực tính toàn vẹn dữ liệu (Kiểm tra chữ ký SecureHash của VNPAY)
            var vnpayData = new SortedList<string, string>(StringComparer.Ordinal);
            string vnp_SecureHash = queries["vnp_SecureHash"]!;

            foreach (var key in queries.Keys)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_") && key != "vnp_SecureHash")
                {
                    vnpayData.Add(key, queries[key]!);
                }
            }

            var stringToHash = new StringBuilder();
            foreach (var kv in vnpayData)
            {
                stringToHash.Append(kv.Key + "=" + Uri.EscapeDataString(kv.Value) + "&");
            }
            if (stringToHash.Length > 0) stringToHash.Remove(stringToHash.Length - 1, 1);

            string checkHash = Utils.HmacSHA512(vnp_HashSecret, stringToHash.ToString());

            if (checkHash != vnp_SecureHash)
            {
                return Ok(new { RspCode = "97", Message = "Invalid Signature" });
            }

            // 2. Phân tích kết quả thanh toán từ các tham số
            Guid bookingId = Guid.Parse(queries["vnp_TxnRef"]!);
            string responseCode = queries["vnp_ResponseCode"]!; // Mã "00" đại diện cho thành công
            string transactionNo = queries["vnp_TransactionNo"]!; // Mã giao dịch của VNPAY
            decimal amount = decimal.Parse(queries["vnp_Amount"]!) / 100; // Chia lại cho 100 để về đúng số tiền VND

            // 3. Tìm kiếm đơn đặt phòng từ DB để đối soát thông tin
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId);
            if (booking == null)
                return Ok(new { RspCode = "01", Message = "Order not found" });

            if (booking.Status != BookingStatus.Pending)
                return Ok(new { RspCode = "02", Message = "Order already confirmed or processed" });

            // 4. Cập nhật trạng thái Booking và ghi nhận thông tin hóa đơn Payment
            if (responseCode == "00")
            {
                booking.Status = BookingStatus.Confirmed;

                var payment = new Payment
                {
                    Id = Guid.NewGuid(),
                    BookingId = booking.Id,
                    Amount = amount,
                    Status = PaymentStatus.Success,
                    PaymentMethod = "VNPAY",
                    TransactionCode = transactionNo,
                    CreatedAt = DateTime.UtcNow
                };

                payment.PaymentDetails.Add(new PaymentDetail
                {
                    Id = Guid.NewGuid(),
                    PaymentId = payment.Id,
                    ItemName = $"Thanh toan online hoa don dat phong khach san: {booking.Id}",
                    Amount = amount,
                    CreatedAt = DateTime.UtcNow
                });

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();
            }
            else
            {
                // Xử lý khi giao dịch thất bại hoặc người dùng chủ động hủy giao dịch trên trang thanh toán
                booking.Status = BookingStatus.Cancelled;
                await _context.SaveChangesAsync();
            }

            // Trả về cấu hình định dạng phản hồi Json theo quy định tài liệu VNPAY
            return Ok(new { RspCode = "00", Message = "Confirm Success" });
        }

        [HttpGet("my-history")]
        [Authorize]
        public async Task<IActionResult> GetMyBookingHistory()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "Vui lòng đăng nhập lại hệ thống." });
            }

            Guid userId = Guid.Parse(userIdClaim.Value);

            var history = await _context.Bookings
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedAt) // Sắp xếp theo ngày tạo đơn mới nhất xuống cũ nhất
                .Select(b => new {
                    b.Id,
                    b.CheckIn,
                    b.CheckOut,
                    b.TotalAmount,
                    Status = b.Status.ToString()
                })
                .ToListAsync();

            return Ok(history);
        }
    }

    // Định dạng DTO nhận yêu cầu xử lý từ giao diện client đưa lên
    public class PaymentRequest
    {
        public Guid BookingId { get; set; }
        public string PaymentMethod { get; set; } = "VNPAY";
    }
}