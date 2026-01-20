using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class OperationalBalanceScoreCardOrgStructureDTO 
    {
        public OperationalBalanceScoreCardOrgStructureDTO()
        {
            Sectors = new List<OrgStructureCardDTO>();
            Sectors_NoPermssion = new List<OrgStructureCardDTO>();
        }
        public decimal Performance { get; set; }
        public decimal AvragePerformance { get; set; }
        public decimal VariancePerformance { get; set; }

        public string Status { get; set; }
        public string AvrageStatus { get; set; }
        public string VarianceStatus { get; set; }
        public OrgStructureCardDTO CorporateSectors { get; set; }

        public List<OrgStructureCardDTO> Sectors { get; set; }
        public List<OrgStructureCardDTO> Sectors_NoPermssion { get; set; }
    }
}
