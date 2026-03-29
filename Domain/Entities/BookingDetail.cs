using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class BookingDetail : BaseEntity
    {
        public Guid BookingId { get; set; }

        public Guid RoomId { get; set; }

        public decimal PricePerNight { get; set; }

        public int Nights { get; set; }

        public Booking Booking { get; set; }

        public Room Room { get; set; }
    }
}
