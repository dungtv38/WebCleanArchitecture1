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
    public class BookingDetailConfiguration : IEntityTypeConfiguration<BookingDetail>
    {
        public void Configure(EntityTypeBuilder<BookingDetail> builder)
        {
            builder.ToTable("BookingDetails");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PricePerNight)
                   .HasPrecision(18, 2);

            builder.Property(x => x.Nights)
                   .IsRequired();

            builder.HasOne(x => x.Booking)
                   .WithMany(b => b.BookingDetails)
                   .HasForeignKey(x => x.BookingId);

            builder.HasOne(x => x.Room)
                   .WithMany(r => r.BookingDetails)
                   .HasForeignKey(x => x.RoomId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
