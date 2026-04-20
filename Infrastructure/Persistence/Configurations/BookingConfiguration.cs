using Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.ToTable("Bookings");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TotalAmount)
                   .HasPrecision(18, 2)
                   .IsRequired();

            builder.Property(x => x.CheckIn)
                   .IsRequired();

            builder.Property(x => x.CheckOut);

            builder.Property(x => x.Status)
                   .HasConversion<int>();   // lưu dạng int

            builder.HasOne(x => x.User)
                   .WithMany(u => u.Bookings)
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Hotel)
                   .WithMany()
                   .HasForeignKey(x => x.HotelId)
                   .OnDelete(DeleteBehavior.Restrict);

   
            builder.HasMany(x => x.BookingDetails)
                   .WithOne(d => d.Booking)
                   .HasForeignKey(d => d.BookingId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasIndex(x => new { x.HotelId, x.CheckIn, x.CheckOut });

            builder.HasIndex(x => x.UserId);
        }
    }
}