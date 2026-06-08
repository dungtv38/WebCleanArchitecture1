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
            // 1. Kiểm tra tính hợp lệ của ngày tháng đặt phòng
            if (request.CheckIn.Date < DateTime.UtcNow.Date)
                throw new Exception("Ngày nhận phòng không hợp lệ.");

            if (request.CheckIn >= request.CheckOut)
                throw new Exception("Ngày trả phòng phải sau ngày nhận phòng.");

            int totalNights = (request.CheckOut.Date - request.CheckIn.Date).Days;

            if (totalNights <= 0)
                throw new Exception("Số đêm không hợp lệ.");

            // Sử dụng Transaction để đảm bảo tính toàn vẹn dữ liệu khi lưu nhiều bảng
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 2. Kiểm tra trùng lịch: Xem các phòng yêu cầu có đang bận trong khoảng thời gian này không
                // 🌟 CẬP NHẬT TRONG BOOKINGSERVICE.CS:
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

                // 3. Lấy thông tin danh sách phòng thực tế từ DB để lấy giá tiền (PricePerNight)
                var rooms = await _context.Rooms
                    .Where(r => request.RoomIds.Contains(r.Id))
                    .ToListAsync();

                if (rooms.Count != request.RoomIds.Count)
                    throw new Exception("Một số phòng không tồn tại trong hệ thống.");

                // 4. Khởi tạo thực thể Booking (Đơn đặt phòng tổng)
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

                // 5. Duyệt từng phòng vật lý để tính tiền lẻ và tạo hóa đơn chi tiết (BookingDetail)
                foreach (var room in rooms)
                {
                    // Thành tiền của 1 phòng = Giá phòng đó * Số đêm lưu trú
                    decimal roomTotalCost = room.PricePerNight * totalNights;
                    totalAmount += roomTotalCost;

                    booking.BookingDetails.Add(new BookingDetail
                    {
                        Id = Guid.NewGuid(),
                        BookingId = booking.Id,
                        RoomId = room.Id,
                        Nights = totalNights,
                        PricePerNight = room.PricePerNight // Lưu lại giá tại thời điểm đặt phòng
                    });
                }

                // Gán tổng tiền của toàn bộ hóa đơn cho Booking
                booking.TotalAmount = totalAmount;

                // 6. Lưu dữ liệu xuống Database
                _context.Bookings.Add(booking);

                var result = await _context.SaveChangesAsync();
                if (result == 0)
                    throw new Exception("Không thể lưu đơn đặt phòng.");

                // Commit transaction thành công
                await transaction.CommitAsync();

                return booking;
            }
            catch (Exception)
            {
                // Nếu có bất kỳ lỗi nào xảy ra, rollback lại toàn bộ dữ liệu tránh rác DB
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}