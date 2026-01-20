using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncJobs.Models
{
    public class WFRequest
    {
        public int ID { get; set; }
        public int RelatedItemID { get; set; }
        public string Status { get; set; }

    }
}
