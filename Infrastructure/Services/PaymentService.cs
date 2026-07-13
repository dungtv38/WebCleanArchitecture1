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

        public async Task<string> CreateOnlinePaymentUrl(
    Guid bookingId,
    string returnUrl)
        {
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(x => x.Id == bookingId);

            if (booking == null)
                throw new Exception("Không tìm thấy booking");


            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                BookingId = bookingId,
                Amount = booking.TotalAmount,
                Status = PaymentStatus.Pending,
                PaymentMethod = "VNPay",
                TransactionCode = $"VNP-{DateTime.UtcNow.Ticks}",
                CreatedAt = DateTime.UtcNow
            };


            _context.Payments.Add(payment);

            await _context.SaveChangesAsync();


            // Demo URL
            var paymentUrl =
                $"{returnUrl}?paymentId={payment.Id}";


            return paymentUrl;
        }

        public async Task<bool> HandleIpnCallback(
      string transactionCode,
      bool isSuccess)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(x =>
                    x.TransactionCode == transactionCode);


            if (payment == null)
                return false;


            if (isSuccess)
            {
                payment.Status = PaymentStatus.Success;


                var booking = await _context.Bookings
                    .FirstAsync(x => x.Id == payment.BookingId);


                booking.Status = BookingStatus.Confirmed;
            }
            else
            {
                payment.Status = PaymentStatus.Failed;
            }


            await _context.SaveChangesAsync();


            return true;
        }

        public async Task<bool> ProcessOfflinePayment(CreateOfflinePaymentDto dto)
        {
            var booking = await _context.Bookings.FindAsync(dto.BookingId);
            if (booking == null) throw new Exception("Không tìm thấy đơn đặt phòng.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var payment = new Payment
                {
                    Id = Guid.NewGuid(),
                    BookingId = dto.BookingId,
                    Amount = dto.Amount,
                    Status = PaymentStatus.Success, // Đảm bảo Enum này khớp với định nghĩa
                    PaymentMethod = "Cash",
                    TransactionCode = $"OFF-{DateTime.UtcNow.Ticks.ToString().Substring(10)}",
                    CreatedAt = DateTime.UtcNow
                };

                var detail = new PaymentDetail
                {
                    Id = Guid.NewGuid(),
                    PaymentId = payment.Id,
                    ItemName = dto.Note ?? "Thanh toán tại quầy",
                    Amount = dto.Amount,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Payments.Add(payment);

                // SỬA Ở ĐÂY: Viết hoa chữ P và D theo đúng DbSet trong DbContext
                _context.PaymentDetails.Add(detail);

                if (dto.Amount >= booking.TotalAmount)
                {
                    booking.Status = BookingStatus.Confirmed;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}

  

