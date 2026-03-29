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

        public DateTime CheckInDate { get; set; }

        public DateTime CheckOutDate { get; set; }

        public decimal TotalAmount { get; set; }

        public User User { get; set; }

        public Hotel Hotel { get; set; }

        public ICollection<BookingDetail> BookingDetails { get; set; }
    }
}
