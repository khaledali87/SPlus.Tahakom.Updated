using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class DelegationDTO
    {
        public int ID { get; set; }
        public UserDTO FromUser { get; set; }
        public UserDTO ToUser { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public UserDTO CreatedBy { get; set; }
        public UserDTO ModifiedBy { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }
}
