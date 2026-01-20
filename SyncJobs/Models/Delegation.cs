using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncJobs.Models
{
    public class Delegation
    {
        public int ID { get; set; }
        public UserInfo FromUser { get; set; }
        public UserInfo ToUser { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public UserInfo CreatedBy { get; set; }
        public UserInfo ModifiedBy { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }
}
