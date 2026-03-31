using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class CreateHotelRequest
    {
        public string Name { get; set; } = default!;
        public string Address { get; set; } = default!;
        public string City { get; set; } = default!;
        public string Description { get; set; } = default!;
        public Guid OwnerId { get; set; }
        public List<string>? ImageUrls { get; set; }

    }
}
