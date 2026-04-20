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

        public async Task<Room> CreateAsync(CreateRoomRequest request)
        {
            var roomTypeExists = await _context.RoomTypes
             .AnyAsync(x => x.Id == request.RoomTypeId);

            if (!roomTypeExists)
                throw new Exception("RoomType not found");

            // 🔥 2. Check trùng RoomNumber
            var isDuplicate = await _context.Rooms
                .AnyAsync(x => x.RoomTypeId == request.RoomTypeId
                            && x.RoomNumber == request.RoomNumber);

            if (isDuplicate)
                throw new Exception("Room number already exists");

            // 🔥 3. Tạo Room
            var room = new Room
            {
                Id = Guid.NewGuid(),
                RoomTypeId = request.RoomTypeId,
                RoomNumber = request.RoomNumber,
                Note = request.Note,
                Status = RoomStatus.Available,
                CreatedAt = DateTime.UtcNow,
                Images = new List<RoomImage>()
            };

            // 🔥 4. Thêm ảnh
            if (request.ImageUrls != null && request.ImageUrls.Any())
            {
                foreach (var url in request.ImageUrls)
                {
                    room.Images.Add(new RoomImage
                    {
                        Id = Guid.NewGuid(),
                        ImageUrl = url,
                        RoomId = room.Id, // ⚠ QUAN TRỌNG
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            return room;
        }

        public async Task<bool> DeleteAsync(Guid roomId)
        {
            var room = await _context.Rooms
            .Include(r => r.Images)
            .FirstOrDefaultAsync(r => r.Id == roomId);

            if (room == null)
                throw new Exception("Room not found");

            // 🔥 Xoá cả ảnh (cascade hoặc manual)
            _context.RoomImages.RemoveRange(room.Images);

            _context.Rooms.Remove(room);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Room>> GetByRoomTypeIdAsync(Guid roomTypeId)
        {
            return await _context.Rooms
            .Where(r => r.RoomTypeId == roomTypeId)
            .Include(r => r.Images)
            .ToListAsync();
        }

        public async Task<bool> UpdateAsync(Guid roomId, UpdateRoomRequest request)
        {
            var room = await _context.Rooms
         .Include(r => r.Images)
         .FirstOrDefaultAsync(r => r.Id == roomId);

            if (room == null)
                throw new Exception("Room not found");

            // 🔥 1. Check trùng RoomNumber
            var isDuplicate = await _context.Rooms
                .AnyAsync(x => x.Id != roomId &&
                               x.RoomTypeId == room.RoomTypeId &&
                               x.RoomNumber == request.RoomNumber);

            if (isDuplicate)
                throw new Exception("Room number already exists");

            // 🔥 2. Update field
            room.RoomNumber = request.RoomNumber;
            room.Note = request.Note;

            // 🔥 3. Update Images
            if (request.ImageUrls != null)
            {
                var existingImages = room.Images.Select(x => x.ImageUrl).ToList();

                // ❌ Remove ảnh không còn
                var imagesToRemove = room.Images
                    .Where(img => !request.ImageUrls.Contains(img.ImageUrl))
                    .ToList();

                _context.RoomImages.RemoveRange(imagesToRemove);

                // ➕ Add ảnh mới
                var imagesToAdd = request.ImageUrls
                    .Where(url => !existingImages.Contains(url));

                foreach (var url in imagesToAdd)
                {
                    room.Images.Add(new RoomImage
                    {
                        Id = Guid.NewGuid(),
                        RoomId = room.Id,
                        ImageUrl = url,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateStatusAsync(Guid roomId, RoomStatus status)
        {
            var room = await _context.Rooms.FindAsync(roomId);

            if (room == null)
                throw new Exception("Room not found");

            room.Status = status;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
