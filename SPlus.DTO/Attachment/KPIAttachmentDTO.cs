using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class KPIAttachmentDTO
    { 
        public int ID { get; set; }
        public int RelatedItemID { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public int Size { get; set; }
        public string Type { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public decimal? Value { get; set; }
        public int Status { get; set; }

    }
}
