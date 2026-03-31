using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Hotel> Hotels => Set<Hotel>();
        public DbSet<RoomType> RoomTypes => Set<RoomType>();
        public DbSet<Room> Rooms => Set<Room>();
        public DbSet<Booking> Bookings => Set<Booking>();
        public DbSet<BookingDetail> BookingDetails => Set<BookingDetail>();
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<HotelImage> HotelImages => Set<HotelImage>();
        public DbSet<RoomImage> RoomImages => Set<RoomImage>();

       
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            var adminId = Guid.Parse("eec4b862-8bba-417c-904a-b926c33a7899");
            var hotelId = Guid.Parse("d72221bf-e4a9-4610-b152-1882ea22fe90");

            // 3. Seed dữ liệu User (Bắt buộc phải có trước để làm Owner)
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = adminId,
                FullName = "System Admin",
                Email = "admin@hotel.com",
                PasswordHash = "hashed_password", // Thực tế nên dùng BCrypt hoặc Identity
                Role = Domain.Entities.UserRole.Admin,
                PhoneNumber="0869075546",
                CreatedAt = DateTime.Now
            });

            // 4. Seed dữ liệu Hotel mẫu (Tùy chọn)
            modelBuilder.Entity<Hotel>().HasData(new Hotel
            {
                Id = hotelId,
                Name = "Grand Central Hotel",
                Description = "Khách sạn trung tâm thành phố",
                Address = "123 Ly Thuong Kiet",
                City = "Hà Nội",
                OwnerId = adminId, // Link tới User ở trên
                CreatedAt = DateTime.Now
            });
        }
    }
}
