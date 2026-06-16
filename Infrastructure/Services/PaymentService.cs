using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Common;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _context;

        public PaymentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> CreatePaymentUrlAsync(Guid bookingId, string paymentMethod, string ipAddress)
        {
            // 1. Kiểm tra đơn đặt phòng
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null)
                throw new Exception("Đơn đặt phòng không tồn tại.");

            if (booking.Status == BookingStatus.Cancelled)
                throw new Exception("Đơn đặt phòng này đã bị hủy, không thể thanh toán.");

            if (booking.Status == BookingStatus.Confirmed)
                throw new Exception("Đơn đặt phòng này đã được xác nhận thanh toán trước đó.");

            // 2. Nếu thanh toán tại quầy (COUNTER) - Xử lý đóng gói luồng luôn
            if (paymentMethod.ToUpper() == "COUNTER")
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    booking.Status = BookingStatus.Confirmed; // Hoặc cứ để Pending tùy quy trình vận hành thực tế

                    var cashPayment = new Payment
                    {
                        Id = Guid.NewGuid(),
                        BookingId = booking.Id,
                        Amount = booking.TotalAmount,
                        Status = PaymentStatus.Success,
                        PaymentMethod = "COUNTER",
                        TransactionCode = $"CASH-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}",
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.Payments.Add(cashPayment);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return "COUNTER_SUCCESS";
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }

           
            string vnp_Url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
            string vnp_TmnCode = "YOUR_TMN_CODE_HERE";    
            string vnp_HashSecret = "YOUR_HASH_SECRET_HERE"; 
            string vnp_ReturnUrl = "http://localhost:3000/payment-return"; 

            // Lưu ý: VNPAY tính số tiền theo đơn vị đồng nhưng loại bỏ phần thập phân bằng cách nhân 100 
            // Ví dụ: 100,000 VND -> truyền sang VNPAY phải thành string "10000000"
            long vnpAmount = (long)(booking.TotalAmount * 100);

            var vnpayData = new SortedList<string, string>(StringComparer.Ordinal)
            {
                { "vnp_Version", "2.1.0" },
                { "vnp_Command", "pay" },
                { "vnp_TmnCode", vnp_TmnCode },
                { "vnp_Amount", vnpAmount.ToString() },
                { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") },
                { "vnp_CurrCode", "VND" },
                { "vnp_IpAddr", string.IsNullOrEmpty(ipAddress) ? "127.0.0.1" : ipAddress },
                { "vnp_Locale", "vn" },
                { "vnp_OrderInfo", $"Thanh toan don hang {booking.Id}" },
                { "vnp_OrderType", "other" },
                { "vnp_ReturnUrl", vnp_ReturnUrl },
                { "vnp_TxnRef", booking.Id.ToString() } // Dùng luôn Id Đơn hàng làm Mã đối soát tham chiếu
            };

            // Tiến hành nối chuỗi tham số theo thứ tự Alphabet để tạo mã băm SecureHash
            var stringToHash = new StringBuilder();
            var urlBuilder = new StringBuilder(vnp_Url + "?");

            foreach (var kv in vnpayData)
            {
                stringToHash.Append(kv.Key + "=" + Uri.EscapeDataString(kv.Value) + "&");
                urlBuilder.Append(kv.Key + "=" + Uri.EscapeDataString(kv.Value) + "&");
            }

            if (stringToHash.Length > 0) stringToHash.Remove(stringToHash.Length - 1, 1);

            // Băm chuỗi dữ liệu với chuỗi khóa bí mật
            string vnp_SecureHash = Utils.HmacSHA512(vnp_HashSecret, stringToHash.ToString());
            urlBuilder.Append("vnp_SecureHash=" + vnp_SecureHash);

            return urlBuilder.ToString();
        }
    }
}