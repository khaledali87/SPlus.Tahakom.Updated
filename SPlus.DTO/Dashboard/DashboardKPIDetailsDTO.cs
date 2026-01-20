using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class DashboardKPIDetailsDTO : BaseLevelDTO
    {
        public decimal Value { get; set; }
        public decimal Target { get; set; }
        public string Polarity { get; set; }
        public string UnitOfMeasure { get; set; }
        public decimal OutOfTarget { get; set; }
        public string Frequency { get; set; }
        public string EnglishUnitDetails { get; set; }
        public string ArabicUnitDetails { get; set; }
        public string Status { get; set; }
        public string Direction { get; set; }
        public DateTime LastUpdate { get; set; }
        public UserListDTO ChampionModel { get; set; }
        public UserListDTO OwnerModel { get; set; }
        public UserListDTO SponsorModel { get; set; }
        public SingularKPITypeDTO KPIType { get; set; }
        public OrgStructureListDTO OrgStructure { get; set; }
        public List<KPIMeasureDTO> KPIMeasures { get; set; }
    }
}
