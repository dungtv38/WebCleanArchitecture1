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
    public class RoomConfiguration : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.ToTable("Rooms");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.RoomNumber)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.HasOne(x => x.RoomType)
                   .WithMany(rt => rt.Rooms)
                   .HasForeignKey(x => x.RoomTypeId);

            builder.HasMany(x => x.BookingDetails)
                   .WithOne(bd => bd.Room)
                   .HasForeignKey(bd => bd.RoomId);
        }
    }
}
