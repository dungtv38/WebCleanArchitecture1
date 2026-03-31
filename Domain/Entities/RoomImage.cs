using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class RoomImage : BaseEntity
    {
        public Guid RoomId { get; set; }

        public string ImageUrl { get; set; } = default!;

        public Room Room { get; set; } = default!;
    }
}
