using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{


    public class Room : BaseEntity
    {
        public Guid RoomTypeId { get; set; }

        public string RoomNumber { get; set; } = default!;


        public RoomStatus Status { get; set; } = RoomStatus.Available;


        public string? Note { get; set; }

        public RoomType RoomType { get; set; } = default!;
        public ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();
        public ICollection<RoomImage> Images { get; set; } = new List<RoomImage>();
    }


    public enum RoomStatus
    {
        Available = 0,     // Trống, sẵn sàng đón khách
        Occupied = 1,      // Đang có khách ở
        Cleaning = 2,      // Đang dọn dẹp
        Maintenance = 3,   // Đang bảo trì/hỏng
        Booked = 4         // Đã được đặt (nhưng khách chưa check-in)
    }



}
