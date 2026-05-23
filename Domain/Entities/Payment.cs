using Domain.Common;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Payment : BaseEntity
    {
        public Guid BookingId { get; set; }
       

        public decimal Amount { get; set; }

        public PaymentStatus Status { get; set; }
            = PaymentStatus.Pending;

        public string PaymentMethod { get; set; } = default!;

        public string? TransactionCode { get; set; }

        // Navigation
        public Booking Booking { get; set; } = default!;

        public ICollection<PaymentDetail> PaymentDetails { get; set; }
            = new List<PaymentDetail>();

    }
}
