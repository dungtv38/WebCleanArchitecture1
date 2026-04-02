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

            
            builder.Property(x => x.Status)
                   .IsRequired()
                   .HasDefaultValue(RoomStatus.Available);

           
            builder.Property(x => x.Note)
                   .HasMaxLength(500);

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

            // Mẹo nâng cao: Đảm bảo số phòng là duy nhất trong cùng một Loại phòng/Khách sạn
            builder.HasIndex(x => new { x.RoomTypeId, x.RoomNumber }).IsUnique();
        }
    }
}
