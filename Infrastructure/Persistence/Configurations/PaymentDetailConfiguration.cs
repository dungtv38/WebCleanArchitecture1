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
    public class PaymentDetailConfiguration
        : IEntityTypeConfiguration<PaymentDetail>
    {
        public void Configure(EntityTypeBuilder<PaymentDetail> builder)
        {
            builder.ToTable("PaymentDetails");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ItemName)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(x => x.Amount)
                   .HasPrecision(18, 2)
                   .IsRequired();

            builder.HasOne(x => x.Payment)
                   .WithMany(p => p.PaymentDetails)
                   .HasForeignKey(x => x.PaymentId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.PaymentId);
        }
    }
}
