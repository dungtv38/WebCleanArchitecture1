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

        public string RoomNumber { get; set; }

        public RoomType RoomType { get; set; }

        public ICollection<BookingDetail> BookingDetails { get; set; }
    }
}
