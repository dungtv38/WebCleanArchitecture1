using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Configurations
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.ToTable("Bookings");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TotalAmount)
                   .HasPrecision(18, 2);

            builder.Property(x => x.CheckInDate)
                   .IsRequired();

            builder.Property(x => x.CheckOutDate)
                   .IsRequired();

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
                   .HasForeignKey(d => d.BookingId);
        }
    }
}
