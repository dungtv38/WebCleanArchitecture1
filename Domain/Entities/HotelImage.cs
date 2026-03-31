using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class HotelImage : BaseEntity
    {
        public Guid HotelId { get; set; }

        public string ImageUrl { get; set; } = default!;

        public bool IsThumbnail { get; set; }

        public Hotel Hotel { get; set; } = default!;
    }
}
