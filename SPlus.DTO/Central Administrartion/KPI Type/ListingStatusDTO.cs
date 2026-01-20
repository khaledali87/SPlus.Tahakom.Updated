using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class ListingStatusDTO : BaseDTO
    {
        public string Code { get; set; }
        public string Color { get; set; }
        public int Order { get; set; }
    }
}
