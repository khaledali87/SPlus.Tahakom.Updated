using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class WFHistoryDTO : BaseDTO
    {
        //public string ArabicStatus { get; set; }
        //public string EnglishStatus { get; set; }

        public int Status { get; set; }

        public UserDTO ActionByModel { get; set; }

        public string Comment { get; set; }
        public string ActionDate { get; set; }
        public string ClosedDate { get; set; }
        public string ActionTime { get; set; }
    }
}
