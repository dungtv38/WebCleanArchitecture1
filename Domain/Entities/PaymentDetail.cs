using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class PaymentDetail :BaseEntity
    {
        public Guid PaymentId { get; set; }

        public string ItemName { get; set; } = default!;

        public decimal Amount { get; set; }

        // Navigation
        public Payment Payment { get; set; } = default!;
    }
}
