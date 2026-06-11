using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class ProcessPaymentRequest
    {
        public Guid BookingId { get; set; }
        public string PaymentMethod { get; set; } // Ví dụ: "VNPAY", "MOMO", "COUNTER" (Tại quầy)
    }
}
