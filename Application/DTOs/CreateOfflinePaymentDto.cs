using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class CreateOfflinePaymentDto
    {
        public Guid BookingId { get; set; }

        public decimal Amount { get; set; }

        public string? Note { get; set; }
    }
}
