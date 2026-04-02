using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class CreateRoomRequest
    {
        public Guid RoomTypeId { get; set; }
        public string RoomNumber { get; set; } = default!;
        public string? Note { get; set; }
        // Bạn có thể thêm list ảnh demo ở đây nếu muốn
        public List<string>? ImageUrls { get; set; }
    }

    public class UpdateRoomStatusRequest
    {
        public RoomStatus Status { get; set; }
    }
}
