using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class RoomConfiguration : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.ToTable("Rooms");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.RoomNumber)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(x => x.Status)
                   .IsRequired()
                   .HasDefaultValue(RoomStatus.Available);

            builder.Property(x => x.Note)
                   .HasMaxLength(500);

            // 🌟 CẤU HÌNH THÊM 2 THUỘC TÍNH MỚI XUỐNG ĐÂY
            builder.Property(x => x.PricePerNight)
                   .IsRequired()
                   .HasPrecision(18, 2); // Đảm bảo lưu đúng định dạng tiền tệ trong SQL (decimal(18,2))

            builder.Property(x => x.MaxGuests)
                   .IsRequired();

            // Các mối quan hệ giữ nguyên
            builder.HasOne(x => x.RoomType)
                   .WithMany(rt => rt.Rooms)
                   .HasForeignKey(x => x.RoomTypeId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.BookingDetails)
                   .WithOne(bd => bd.Room)
                   .HasForeignKey(bd => bd.RoomId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Images)
                   .WithOne(i => i.Room)
                   .HasForeignKey(i => i.RoomId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.RoomTypeId, x.RoomNumber }).IsUnique();
        }
    }
}