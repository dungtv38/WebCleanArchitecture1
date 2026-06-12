using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly AppDbContext _context;
        public BookingController(AppDbContext context, IBookingService bookingService)
        {
            _context = context;
            _bookingService = bookingService;
        }



        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                                  ?? User.FindFirst("id")?.Value;

                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized(new { message = "Không tìm thấy thông tin người dùng." });

                Guid userId = Guid.Parse(userIdClaim);

                // Chạy logic lưu DB thành công
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
                    // Chỉ lấy các thông tin phòng cần thiết, tuyệt đối không chấm ngược lại .Booking
                    Rooms = booking.BookingDetails.Select(bd => new {
                        RoomId = bd.RoomId,
                        Nights = bd.Nights,
                        PricePerNight = bd.PricePerNight
                    }).ToList()
                };

                // Trả về DTO đã bóc tách sạch sẽ
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
            // Tìm đơn đặt phòng kèm theo thông tin phòng để lấy giá tiền
            var booking = await _context.Bookings
                .Include(b => b.BookingDetails)
                    .ThenInclude(br => br.Room)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return NotFound(new { message = "❌ Không tìm thấy đơn đặt phòng hợp lệ." });
            }

            // Tính toán số đêm lưu trú
            int totalNights = (booking.CheckOut - booking.CheckIn).Days;
            if (totalNights <= 0) totalNights = 1; // Đảm bảo tối thiểu tính giá 1 đêm

            // Tính tổng tiền bằng cách cộng giá của tất cả các phòng đã chọn trong đơn hàng
            decimal totalAmount = booking.BookingDetails.Sum(br => br.Room.PricePerNight) * totalNights;

            // Trả về dữ liệu phẳng sạch sẽ cho Frontend React dễ dùng
            var response = new
            {
                Id = booking.Id,
                HotelId = booking.HotelId,
                CheckIn = booking.CheckIn,
                CheckOut = booking.CheckOut,
                TotalAmount = totalAmount,
                Status = booking.Status.ToString() // Trả về dạng chuỗi: "Pending", "Success",...
            };

            return Ok(response);
        }
        [HttpPost("process")]
        public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequest request)
        {
            // 1. Tìm đơn đặt phòng tương ứng dưới Database
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == request.BookingId);
            if (booking == null)
            {
                return NotFound(new { message = "Không tìm thấy đơn đặt phòng tương ứng." });
            }

            // 2. Cập nhật trạng thái từ Pending (0) sang Confirmed (1) dựa theo Enum của bạn
            booking.Status = BookingStatus.Confirmed;

            // 3. Tạo một bản ghi hóa đơn mới lưu vào bảng Payments để lưu vết lịch sử giao dịch
            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                BookingId = booking.Id,
                PaymentMethod = request.PaymentMethod, // Nhận từ body lên (ví dụ: "vietqr")
                Status = Domain.Enums.PaymentStatus.Success,
                CreatedAt = DateTime.UtcNow
            };

            _context.Payments.Add(payment);

            // 4. Lưu đồng thời thay đổi của cả 2 bảng xuống SQL Server
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Thanh toán qua cổng trực tuyến thành công!",
                paymentId = payment.Id,
                status = "Success"
            });
        }

        // Lớp DTO nhận dữ liệu body truyền lên từ Swagger/React gửi sang
        public class PaymentRequest
        {
            public Guid BookingId { get; set; }
            public string PaymentMethod { get; set; } = default!;
        }
        [HttpGet("my-history")]
        [Authorize] // Ép buộc phải có Token JWT hợp lệ gửi kèm trên Header mới cho vào
        public async Task<IActionResult> GetMyBookingHistory()
        {
            // Tìm UserId nằm ẩn trong các Claim của Token JWT gửi từ React lên
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "Vui lòng đăng nhập lại hệ thống." });
            }

            Guid userId = Guid.Parse(userIdClaim.Value);

            // Lấy toàn bộ danh sách đơn hàng của riêng User này xếp từ mới nhất xuống cũ nhất
            var history = await _context.Bookings
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.Id) // Hoặc CreatedAt nếu bảng của bạn có cột thời gian tạo
                .Select(b => new {
                    b.Id,
                    b.CheckIn,
                    b.CheckOut,
                    Status = b.Status.ToString()
                })
                .ToListAsync();

            return Ok(history);
        }


    }
}