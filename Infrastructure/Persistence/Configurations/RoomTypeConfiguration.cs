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
    public class RoomTypeConfiguration : IEntityTypeConfiguration<RoomType>
    {
        public void Configure(EntityTypeBuilder<RoomType> builder)
        {
            builder.ToTable("RoomTypes");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                   .IsRequired()
                   .HasMaxLength(100);

          

            builder.HasOne(x => x.Hotel)
                   .WithMany(h => h.RoomTypes)
                   .HasForeignKey(x => x.HotelId);

            builder.HasMany(x => x.Rooms)
                   .WithOne(r => r.RoomType)
                   .HasForeignKey(r => r.RoomTypeId);
        }
    }
}
