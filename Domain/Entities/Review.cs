using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Review : BaseEntity
    {
        public Guid UserId { get; set; }

        public Guid HotelId { get; set; }

        public int Rating { get; set; }

        public string Comment { get; set; }

        public User User { get; set; }

        public Hotel Hotel { get; set; }
    }
}
