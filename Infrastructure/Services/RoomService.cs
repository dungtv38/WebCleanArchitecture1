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

        public async Task<RoomResponse> CreateAsync(CreateRoomRequest request)
        {
            var roomTypeExists = await _context.RoomTypes
                .AnyAsync(x => x.Id == request.RoomTypeId);

            if (!roomTypeExists)
                throw new Exception("RoomType not found");

            // 🔥 Check trùng RoomNumber
            var isDuplicate = await _context.Rooms
                .AnyAsync(x => x.RoomTypeId == request.RoomTypeId
                            && x.RoomNumber == request.RoomNumber);

            if (isDuplicate)
                throw new Exception("Room number already exists");

            // 🔥 Tạo Room (Bổ sung gán Price và MaxGuests)
            var room = new Room
            {
                Id = Guid.NewGuid(),
                RoomTypeId = request.RoomTypeId,
                RoomNumber = request.RoomNumber,
                Note = request.Note,
                Status = RoomStatus.Available,
                PricePerNight = request.PricePerNight, // 🌟 THÊM MỚI
                MaxGuests = request.MaxGuests,         // 🌟 THÊM MỚI
                CreatedAt = DateTime.UtcNow,
                Images = new List<RoomImage>()
            };

            // 🔥 Thêm ảnh
            if (request.ImageUrls != null && request.ImageUrls.Any())
            {
                foreach (var url in request.ImageUrls)
                {
                    room.Images.Add(new RoomImage
                    {
                        Id = Guid.NewGuid(),
                        ImageUrl = url,
                        RoomId = room.Id,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            // Trả về Response kèm thông tin giá và sức chứa mới
            return new RoomResponse
            {
                Id = room.Id,
                RoomTypeId = room.RoomTypeId,
                RoomNumber = room.RoomNumber,
                Note = room.Note,
                Status = room.Status.ToString(),
                PricePerNight = room.PricePerNight, // 🌟 THÊM MỚI
                MaxGuests = room.MaxGuests,         // 🌟 THÊM MỚI
                Images = room.Images.Select(i => i.ImageUrl).ToList()
            };
        }

        public async Task<bool> DeleteAsync(Guid roomId)
        {
            var room = await _context.Rooms
                .Include(r => r.Images)
                .FirstOrDefaultAsync(r => r.Id == roomId);

            if (room == null)
                throw new Exception("Room not found");

            _context.RoomImages.RemoveRange(room.Images);
            _context.Rooms.Remove(room);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<RoomResponse?> GetByRoomTypeIdAsync(Guid roomId)
        {
            return await _context.Rooms
                .Where(r => r.Id == roomId)
                .Select(r => new RoomResponse
                {
                    Id = r.Id,
                    RoomTypeId = r.RoomTypeId,
                    RoomNumber = r.RoomNumber,
                    Note = r.Note,
                    Status = r.Status.ToString(), // Nếu Status ở Entity là Enum thì thêm .ToString()
                    PricePerNight = r.PricePerNight,
                    MaxGuests = r.MaxGuests,

                    // Map danh sách ảnh thành danh sách chuỗi URL, triệt tiêu hoàn toàn Object Cycle
                    Images = r.Images.Select(img => img.ImageUrl).ToList()
                })
                .FirstOrDefaultAsync();
        }
        public async Task<bool> UpdateAsync(Guid roomId, UpdateRoomRequest request)
        {
            var room = await _context.Rooms
                .Include(r => r.Images)
                .FirstOrDefaultAsync(r => r.Id == roomId);

            if (room == null)
                throw new Exception("Room not found");

            // 🔥 Check trùng RoomNumber
            var isDuplicate = await _context.Rooms
                .AnyAsync(x => x.Id != roomId &&
                               x.RoomTypeId == room.RoomTypeId &&
                               x.RoomNumber == request.RoomNumber);

            if (isDuplicate)
                throw new Exception("Room number already exists");

            // 🔥 Update fields (Cho phép cập nhật cả Price và MaxGuests)
            room.RoomNumber = request.RoomNumber;
            room.Note = request.Note;
            room.PricePerNight = request.PricePerNight; // 🌟 THÊM MỚI
            room.MaxGuests = request.MaxGuests;         // 🌟 THÊM MỚI

            // 🔥 Update Images
            if (request.ImageUrls != null)
            {
                var existingImages = room.Images.Select(x => x.ImageUrl).ToList();

                var imagesToRemove = room.Images
                    .Where(img => !request.ImageUrls.Contains(img.ImageUrl))
                    .ToList();

                _context.RoomImages.RemoveRange(imagesToRemove);

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