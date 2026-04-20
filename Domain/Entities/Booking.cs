using Domain.Common;
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

        public Hotel Hotel { get; set; } = default!;

        public ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();
    }
    public enum BookingStatus
    {
        Pending = 0,
        Confirmed = 1,
        Cancelled = 2
    }
}
