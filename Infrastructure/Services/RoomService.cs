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
    public class RoomService : IRoomService

    {
        private readonly AppDbContext _context;
        public RoomService(AppDbContext context) => _context = context;

        public async Task<Room> CreateAsync(CreateRoomRequest request, Guid currentUserId)
        {
            var roomType = await _context.RoomTypes
            .Include(rt => rt.Hotel)
            .FirstOrDefaultAsync(rt => rt.Id == request.RoomTypeId);

            if (roomType == null || roomType.Hotel.OwnerId != currentUserId)
            {
                throw new Exception("Bạn không có quyền quản lý khách sạn này.");
            }

            // 2. Kiểm tra xem số phòng đã tồn tại trong loại phòng này chưa
            var isExisted = await _context.Rooms
                .AnyAsync(r => r.RoomTypeId == request.RoomTypeId && r.RoomNumber == request.RoomNumber);

            if (isExisted) throw new Exception("Số phòng này đã tồn tại.");

            // 3. Khởi tạo Room
            var room = new Room
            {
                Id = Guid.NewGuid(),
                RoomTypeId = request.RoomTypeId,
                RoomNumber = request.RoomNumber,
                Status = RoomStatus.Available,
                CreatedAt = DateTime.Now,
                Images = new List<RoomImage>()
            };

            // 4. Thêm ảnh nếu có
            if (request.ImageUrls != null)
            {
                foreach (var url in request.ImageUrls)
                {
                    room.Images.Add(new RoomImage
                    {
                        Id = Guid.NewGuid(),
                        ImageUrl = url,
                        CreatedAt = DateTime.Now
                    });
                }
            }

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
            return room;
        }

        public Task<bool> DeleteAsync(Guid roomId, Guid currentUserId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Room>> GetByRoomTypeIdAsync(Guid roomTypeId)
        {
            return await _context.Rooms
            .Where(r => r.RoomTypeId == roomTypeId)
            .Include(r => r.Images)
            .ToListAsync();
        }

        public Task<bool> UpdateStatusAsync(Guid roomId, RoomStatus status, Guid currentUserId)
        {
            throw new NotImplementedException();
        }
    }
}
