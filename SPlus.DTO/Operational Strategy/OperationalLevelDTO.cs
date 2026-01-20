using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class OperationalLevelDTO : BaseDTO
    {
        public Guid ProjectUID { get; set; }
        public UserListDTO ManagerModel { get; set; }
        public decimal Progress { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string Status { get; set; }
    }
}
