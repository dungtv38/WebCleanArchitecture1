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
    public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
    {
        public void Configure(EntityTypeBuilder<Hotel> builder)
        {
            builder.ToTable("Hotels");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(x => x.Description)
                   .HasMaxLength(1000);

            builder.Property(x => x.Address)
                   .HasMaxLength(300);

            builder.Property(x => x.City)
                   .HasMaxLength(100);

            builder.HasOne(x => x.Owner)
                   .WithMany()
                   .HasForeignKey(x => x.OwnerId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.RoomTypes)
                   .WithOne(rt => rt.Hotel)
                   .HasForeignKey(rt => rt.HotelId);

            builder.HasMany(x => x.Images)
                    .WithOne(img => img.Hotel)
                    .HasForeignKey(img => img.HotelId)
                    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
