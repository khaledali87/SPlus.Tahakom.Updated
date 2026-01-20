using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
   public class CRDashboardDTO
    {
        public int TotlalCount { get; set; }
        public int RejectedCount { get; set; }

        public List<DistributionDataByStatus> CRByStatuses { get; set; }
        public List<DistributionDataByChangeType> CRByType { get; set; }

        public List<DistributionDataByDepartment> CRByDepartment { get; set; }
    }
    public class DistributionDataByChangeType
    {
        public int Count { get; set; }
        public int ChangeTypeId { get; set; }
        public string ArabicName { get; set; }
        public string EnglishName { get; set; }
    }

    public class DistributionDataByDepartment
    {
        public int Count { get; set; }
        public int DepartmentId { get; set; }
        public string ArabicName { get; set; }
        public string EnglishName { get; set; }
    }
}
