using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
            // 1. Kiểm tra ngày đặt hợp lệ
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
                // 2. Kiểm tra trùng lịch phòng (Tránh đặt trùng)
                var occupiedRoomIds = await _context.BookingDetails
                    .Where(bd =>
                        request.RoomIds.Contains(bd.RoomId) &&
                        bd.Booking.Status != BookingStatus.Cancelled &&
                        bd.Booking.CheckIn.Date < request.CheckOut.Date &&
                        bd.Booking.CheckOut.Date > request.CheckIn.Date)
                    .Select(bd => bd.RoomId)
                    .ToListAsync();

                if (occupiedRoomIds.Any())
                    throw new Exception("Một số phòng bạn chọn đã được đặt trong thời gian này.");

                
                var rooms = await _context.Rooms
                    .Include(r => r.RoomType) 
                    .Where(r => request.RoomIds.Contains(r.Id) && r.RoomType.HotelId == request.HotelId)
                    .ToListAsync();

            
                if (rooms.Count != request.RoomIds.Count)
                    throw new Exception("Một số phòng không tồn tại hoặc không thuộc khách sạn này.");

                var booking = new Booking
                {
                    Id = Guid.NewGuid(), // Kế thừa từ BaseEntity (Guid)
                    UserId = userId,
                    HotelId = request.HotelId,
                    CheckIn = request.CheckIn,
                    CheckOut = request.CheckOut,
                    Status = BookingStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    BookingDetails = new List<BookingDetail>()
                };

                decimal totalAmount = 0;

                // 5. Thêm chi tiết đặt phòng
                foreach (var room in rooms)
                {
                    decimal roomTotalCost = room.PricePerNight * totalNights;
                    totalAmount += roomTotalCost;

                    booking.BookingDetails.Add(new BookingDetail
                    {
                        Id = Guid.NewGuid(), // Kế thừa từ BaseEntity (Guid)
                        BookingId = booking.Id,
                        RoomId = room.Id,
                        Nights = totalNights,
                        PricePerNight = room.PricePerNight,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }

                booking.TotalAmount = totalAmount;

                // 6. Lưu xuống DB
                _context.Bookings.Add(booking);

                var result = await _context.SaveChangesAsync();
                if (result == 0)
                    throw new Exception("Không thể lưu đơn đặt phòng.");

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