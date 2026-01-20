using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class KPICommentDTO
    {
        public int ID { get; set; }
        public int KPIID { get; set; }
        public string Comment { get; set; }
        public string CreatedBy { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public UserListDTO CreatedByModel { get; set; }

    }
}
