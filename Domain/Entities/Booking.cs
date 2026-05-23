using Domain.Common;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Booking : BaseEntity
    {
        public Guid UserId { get; set; }

        public Guid HotelId { get; set; }

        public DateTime CheckIn { get; set; }

        public DateTime CheckOut { get; set; }

        public decimal TotalAmount { get; set; }

        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        // Navigation
        public User User { get; set; } = default!;
        public ICollection<Payment> Payments { get; set; }
            = new List<Payment>();

        public Hotel Hotel { get; set; } = default!;

        public ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();
    }
    
}
