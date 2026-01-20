using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class StrategicInitiativeDTO : BaseDTO
    {
        public Guid ProjectUID { get; set; }
        public string EnglishDescription { get; set; }
        public string ArabicDescription { get; set; }
        public string Status { get; set; }
        public decimal Progress { get; set; }
        public UserListDTO ManagerModel { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
