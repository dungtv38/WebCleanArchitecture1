using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class AvailableRoomDto
    {
        public Guid RoomId { get; set; }
        public string RoomName { get; set; }
        public decimal PricePerNight { get; set; }
        public string RoomType { get; set; }
    }
}
