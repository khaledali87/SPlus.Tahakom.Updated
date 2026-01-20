using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class SingularKPIDTO : BaseLevelDTO
    {
        public string EnglishDescription { get; set; }
        public string ArabicDescription { get; set; }
        public string EnglishEquation { get; set; }
        public string ArabicEquation { get; set; }
        public string Polarity { get; set; }
        public string UnitOfMeasure { get; set; }
        public string EnglishUnitDetails { get; set; }
        public int CalculationMethod { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime BaselineDate { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public string Years { get; set; }
        public string Frequency { get; set; }
        public string DataSource { get; set; }
        public string Formula { get; set; }
        public decimal Baseline { get; set; }
        public string ArabicUnitDetails { get; set; }
        public string ReferenceNumber { get; set; }
        public decimal Weight { get; set; }
        public bool IsLocked { get; set; }
        public bool RequireUpdate { get; set; }
        public bool CanUpdate { get; set; }
        public DateTime? UnlockDate { get; set; }
        public KPITypeDTO KPIType { get; set; }
        public UserDTO ChampionModel { get; set; }
        public UserDTO OwnerModel { get; set; }
        public UserDTO SponsorModel { get; set; }
        public SingularStrategicObjectiveDTO StrategicObjective { get; set; }
        public OrgStructureDTO Division { get; set; } 
        public OrgStructureDTO OrgStructure { get; set; }
        public List<KPIMeasureDTO> KPIMeasures { get; set; } 
        public List<KPICommentDTO> KPIComments { get; set; }
        public List<AttachmentDTO> Attachments { get; set; }
        public List<ParameterDTO> Parameters { get; set; }
    }

}
