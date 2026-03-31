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
    public class RoomImageConfiguration : IEntityTypeConfiguration<RoomImage>
    {
        public void Configure(EntityTypeBuilder<RoomImage> builder)
        {
            builder.ToTable("RoomImages");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ImageUrl)
                   .IsRequired()
                   .HasMaxLength(500); // Đảm bảo URL không quá dài hoặc bị cắt cụt

            // Quan hệ ngược lại từ RoomImage về Room
            builder.HasOne(x => x.Room)
                   .WithMany(r => r.Images)
                   .HasForeignKey(x => x.RoomId);
        }
    }
}
