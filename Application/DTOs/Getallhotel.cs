using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class Getallhotel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<string> Images
        {
            get; set;
        }
        public string Address {  get; set; }
        public string Description {  get; set; }

    }
}
