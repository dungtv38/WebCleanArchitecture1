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
    internal class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");

            builder.HasKey(x => x.Id);

            // Amount
            builder.Property(x => x.Amount)
                   .HasPrecision(18, 2)
                   .IsRequired();

            // Method
            builder.Property(x => x.PaymentMethod)
                   .HasMaxLength(50)
                   .IsRequired();

            // Transaction code
            builder.Property(x => x.TransactionCode)
                   .HasMaxLength(100);

            // Status enum
            builder.Property(x => x.Status)
                   .HasConversion<int>()
                   .IsRequired();

            // Booking relation
            builder.HasOne(x => x.Booking)
                   .WithMany(b => b.Payments)
                   .HasForeignKey(x => x.BookingId)
                   .OnDelete(DeleteBehavior.Restrict);

            // PaymentDetails relation
            builder.HasMany(x => x.PaymentDetails)
                   .WithOne(pd => pd.Payment)
                   .HasForeignKey(pd => pd.PaymentId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Index
            builder.HasIndex(x => x.BookingId);
            builder.HasIndex(x => x.TransactionCode);
        }
    }
}
