using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Hotel : BaseEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public Guid OwnerId { get; set; }

        public User Owner { get; set; }

        public ICollection<RoomType> RoomTypes { get; set; }
    }
}
