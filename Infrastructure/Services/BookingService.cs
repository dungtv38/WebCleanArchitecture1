using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class BookingService : IBookingService
    {
        private readonly AppDbContext _context;

        public BookingService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Booking> CreateAsync(Guid userId, CreateBookingRequest request)
        {
            if (request.CheckIn.Date < DateTime.UtcNow.Date)
                throw new Exception("Ngày nhận phòng không hợp lệ.");

            if (request.CheckIn >= request.CheckOut)
                throw new Exception("Ngày trả phòng phải sau ngày nhận phòng.");

            int totalNights = (request.CheckOut.Date - request.CheckIn.Date).Days;

            if (totalNights <= 0)
                throw new Exception("Số đêm không hợp lệ.");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Check phòng trùng (fix tránh null navigation)
                var occupiedRoomIds = await _context.BookingDetails
                    .Where(bd =>
                        request.RoomIds.Contains(bd.RoomId) &&
                        bd.Booking.Status != BookingStatus.Cancelled &&
                        bd.Booking.CheckIn < request.CheckOut &&
                        bd.Booking.CheckOut > request.CheckIn)
                    .Select(bd => bd.RoomId)
                    .ToListAsync();

                if (occupiedRoomIds.Any())
                    throw new Exception("Một số phòng bạn chọn đã được đặt trong thời gian này.");

                // 2. Load rooms
                var rooms = await _context.Rooms
                    .Include(r => r.RoomType)
                    .Where(r => request.RoomIds.Contains(r.Id))
                    .ToListAsync();

                if (rooms.Count != request.RoomIds.Count)
                    throw new Exception("Một số phòng không tồn tại.");

                // 3. Create booking
                var booking = new Booking
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    HotelId = request.HotelId,
                    CheckIn = request.CheckIn,
                    CheckOut = request.CheckOut,
                    Status = BookingStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    BookingDetails = new List<BookingDetail>()
                };

                decimal totalAmount = 0;

                // 4. Create details
                foreach (var room in rooms)
                {
                    var price = room.RoomType.PricePerNight;

                    booking.BookingDetails.Add(new BookingDetail
                    {
                        Id = Guid.NewGuid(),
                        BookingId = booking.Id,
                        RoomId = room.Id,
                        PricePerNight = price,
                        Nights = totalNights
                    });

                    totalAmount += price * totalNights;
                }

                booking.TotalAmount = totalAmount;

                // 5. SAVE
                _context.Bookings.Add(booking);

                var result = await _context.SaveChangesAsync();
                if (result == 0)
                    throw new Exception("Không thể lưu booking.");

                await transaction.CommitAsync();

                return booking;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}

