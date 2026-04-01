using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class CreateRoomTypeRequest
    {
        public Guid HotelId { get; set; }
        public string Name { get; set; } = default!;
        public decimal PricePerNight { get; set; }
        public int MaxGuests { get; set; }
    }
    public class UpdateRoomTypeRequest
    {
        public string Name { get; set; } = default!;
        public decimal PricePerNight { get; set; }
        public int MaxGuests { get; set; }
    }

}
