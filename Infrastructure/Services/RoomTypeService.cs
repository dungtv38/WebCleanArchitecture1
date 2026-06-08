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
    public class RoomTypeService : IRoomTypeService
    {
        private readonly AppDbContext _context;
        public RoomTypeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(CreateRoomTypeRequest request)
        {
            var hotelExists = await _context.Hotels
                .AnyAsync(x => x.Id == request.HotelId);

            if (!hotelExists)
                throw new Exception("Hotel not found");

            var roomType = new RoomType
            {
                Id = Guid.NewGuid(),
                HotelId = request.HotelId,
                Name = request.Name,
                CreatedAt = DateTime.UtcNow
            };

            _context.RoomTypes.Add(roomType);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var roomType = await _context.RoomTypes.FindAsync(id);
            if (roomType == null)
                throw new Exception("RoomType not found");

            _context.RoomTypes.Remove(roomType);
            await _context.SaveChangesAsync();
        }

        public async Task<List<RoomType>> GetByHotelIdAsync(Guid hotelId)
        {
            // 🌟 SỬA LỖI LOGIC: Phải lọc theo HotelId chứ không phải x.Id (RoomTypeId)
            return await _context.RoomTypes
                .Where(x => x.HotelId == hotelId)
                .ToListAsync();
        }

        public async Task UpdateAsync(Guid id, UpdateRoomTypeRequest request)
        {
            var roomType = await _context.RoomTypes.FindAsync(id);

            if (roomType == null)
                throw new Exception("RoomType not found");

            roomType.Name = request.Name;

            await _context.SaveChangesAsync();
        }
    }
}