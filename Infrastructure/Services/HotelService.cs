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
    public class HotelService : IHotelService
    {
        private readonly AppDbContext _context;
        public HotelService(AppDbContext context)
        {
            _context = context;
        }
        public async Task CreateAsync(CreateHotelRequest request)
        {
            var hotel = new Hotel
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Address = request.Address,
                City = request.City,
                Description = request.Description,
                CreatedAt = DateTime.Now,
                OwnerId = request.OwnerId,
                Images = new List<HotelImage>()
            };
            if (request.ImageUrls != null && request.ImageUrls.Any())
            {
                foreach (var url in request.ImageUrls)
                {
                    var hotelImage = new HotelImage
                    {
                        Id = Guid.NewGuid(),
                        ImageUrl = url,
                        // Giả định cái ảnh đầu tiên sẽ là Thumbnail
                        IsThumbnail = (url == request.ImageUrls.First()),
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                        // LƯU Ý: Không cần gán HotelId ở đây, EF Core tự làm
                    };

                    hotel.Images.Add(hotelImage);
                }


                _context.Hotels.Add(hotel);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null) return false;

            _context.Hotels.Remove(hotel);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<Hotel>> GetAllAsync()
        {
            return await _context.Hotels.ToListAsync();
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateHotelRequest request)
        {
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null) return false;

            hotel.Name = request.Name;
            hotel.Address = request.Address;
            hotel.City = request.City;
            hotel.Description = request.Description;
            hotel.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
    }
  
}
