using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class RoomType : BaseEntity
    {
        public Guid HotelId { get; set; }

        public string Name { get; set; }

        public decimal PricePerNight { get; set; }

        public int MaxGuests { get; set; }

        public Hotel Hotel { get; set; }

        public ICollection<Room> Rooms { get; set; }
    }
}
