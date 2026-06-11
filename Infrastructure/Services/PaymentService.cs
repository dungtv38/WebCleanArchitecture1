using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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

        public async Task<Payment> ProcessPaymentAsync(ProcessPaymentRequest request)
        {
            // 1. Kiểm tra đơn đặt phòng (Booking) có tồn tại không
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == request.BookingId);

            if (booking == null)
                throw new Exception("Đơn đặt phòng không tồn tại.");

            if (booking.Status == BookingStatus.Cancelled)
                throw new Exception("Đơn đặt phòng này đã bị hủy, không thể thanh toán.");

            // Bọc bằng Transaction để đảm bảo an toàn dữ liệu khi lưu cả Payment và Update Booking
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 2. Khởi tạo bản ghi Payment khớp 100% với thuộc tính thực thể
                var payment = new Payment
                {
                    Id = Guid.NewGuid(),
                    BookingId = booking.Id,
                    Amount = booking.TotalAmount,
                    PaymentMethod = request.PaymentMethod,
                    TransactionCode = request.PaymentMethod.ToUpper() == "COUNTER"
                        ? $"CASH-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}"
                        : $"ONLINE-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    PaymentDetails = new List<PaymentDetail>()
                };

                // 3. Xử lý logic trạng thái (Khớp hoàn toàn với Enum PaymentStatus của bạn)
                if (request.PaymentMethod.ToUpper() == "COUNTER")
                {
                    // Thanh toán tại quầy: Trạng thái Payment thu tiền mặt là Pending (0)
                    payment.Status = PaymentStatus.Pending;
                    booking.Status = BookingStatus.Pending;
                }
                else
                {
                    // Giả lập thanh toán qua cổng trực tuyến thành công: Trạng thái là Success (1)
                    payment.Status = PaymentStatus.Success;
                    booking.Status = BookingStatus.Confirmed; 
                }

                // 4. Lưu dữ liệu chi tiết vào bảng PaymentDetail
                payment.PaymentDetails.Add(new PaymentDetail
                {
                    Id = Guid.NewGuid(),
                    PaymentId = payment.Id,
                    ItemName = $"Thanh toán tiền đặt phòng cho đơn hàng {booking.Id.ToString().Substring(0, 8)}",
                    Amount = booking.TotalAmount,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });

                // 5. Thêm mới bản ghi thanh toán và Cập nhật trạng thái phòng đơn đặt
                _context.Payments.Add(payment);
                _context.Bookings.Update(booking);

                // Thực hiện lưu thay đổi xuống SQL Server
                var result = await _context.SaveChangesAsync();
                if (result == 0)
                    throw new Exception("Không thể xử lý dữ liệu thanh toán.");

                // Xác nhận giao dịch thành công hoàn toàn
                await transaction.CommitAsync();

                return payment;
            }
            catch (Exception)
            {
                // Hoàn tác dữ liệu nếu có bất kỳ lỗi phát sinh trong khối try
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}