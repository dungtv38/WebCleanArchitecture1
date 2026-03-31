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
    public class HotelImageConfiguration : IEntityTypeConfiguration<HotelImage>
    {
        public void Configure(EntityTypeBuilder<HotelImage> builder)
        {
            builder.ToTable("HotelImages");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ImageUrl)
                   .IsRequired()
                   .HasMaxLength(500); 

            builder.Property(x => x.IsThumbnail)
                   .HasDefaultValue(false);

           
            builder.HasOne(x => x.Hotel)
                   .WithMany(h => h.Images)
                   .HasForeignKey(x => x.HotelId);
        }
    }
}
