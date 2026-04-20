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

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 2. Thuật toán kiểm tra phòng trống (Overlap Check)
                var occupiedRoomIds = await _context.BookingDetails
                    .Where(bd => request.RoomIds.Contains(bd.RoomId) &&
                                 bd.Booking.Status != BookingStatus.Cancelled &&
                                 bd.Booking.CheckIn < request.CheckOut &&
                                 bd.Booking.CheckOut > request.CheckIn)
                    .Select(bd => bd.RoomId)
                    .ToListAsync();

                if (occupiedRoomIds.Any())
                    throw new Exception("Một số phòng bạn chọn đã được đặt trong thời gian này.");

                // 3. Lấy thông tin phòng và giá
                var rooms = await _context.Rooms
                    .Include(r => r.RoomType)
                    .Where(r => request.RoomIds.Contains(r.Id))
                    .ToListAsync();

                // 4. Tạo đối tượng Booking
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

                // 5. Tạo chi tiết từng phòng (BookingDetail)
                foreach (var room in rooms)
                {
                    var price = room.RoomType.PricePerNight;
                    var subTotal = price * totalNights;

                    booking.BookingDetails.Add(new BookingDetail
                    {
                        Id = Guid.NewGuid(),
                        RoomId = room.Id,
                        PricePerNight = price, // Snapshot giá
                        Nights = totalNights
                    });
                    totalAmount += subTotal;
                }

                booking.TotalAmount = totalAmount;

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return booking;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}

