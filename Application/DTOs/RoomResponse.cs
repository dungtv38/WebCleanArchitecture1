using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class RoomResponse
    {
        public Guid Id { get; set; }
        public Guid RoomTypeId { get; set; }
        public string RoomNumber { get; set; } = default!;
        public string? Note { get; set; }
        public string Status { get; set; } = default!;

        public List<string> Images { get; set; } = new();
    }
}
