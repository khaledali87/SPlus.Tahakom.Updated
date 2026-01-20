using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class KPIDetailsDTO : BaseLevelDTO
    {
        public string EnglishDescription { get; set; }
        public string ArabicDescription { get; set; }
        public string EnglishEquation { get; set; }
        public string ArabicEquation { get; set; }
        public string Polarity { get; set; }
        public string UnitOfMeasure { get; set; }
        public string EnglishUnitDetails { get; set; }
        public string ArabicUnitDetails { get; set; }
        public int CalculationMethod { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public DateTime BaselineDate { get; set; }
        public int Years { get; set; }
        public string Frequency { get; set; }
        public string DataSource { get; set; }
        public string Formula { get; set; }
        public decimal Baseline { get; set; }
        public string Source { get; set; }
        public string Code { get; set; }
        public string Direction { get; set; }
        public bool IsLocked { get; set; }
        public DateTime? UnlockDate { get; set; }
        public bool RequireUpdate { get; set; }
        public bool CanUpdate { get; set; }
        public bool ManualUnLock { get; set; }
        public bool AllowLock { get; set; }
        public string Status { get; set; }
        public decimal OutOfTarget { get; set; }
        public decimal AccumulutiveOutOfTarget { get; set; }
        public decimal Target { get; set; }
        public bool IsConstantTargets { get; set; }
        public decimal? MaxTarget { get; set; }
        public decimal Value { get; set; }
        public int KPITypeID { get; set; }
        public string KPISource { get; set; }
        public decimal Weight { get; set; }
        public string AccumulutiveStatus { get; set; }
        public decimal DepartmentalWeight { get; set; }
        #region Relation Models
        public SingularKPITypeDTO KPIType { get; set; }
        public UserListDTO ChampionModel { get; set; }
        public UserListDTO OwnerModel { get; set; }
        public CAStrategicObjectiveDTO StrategicObjective { get; set; }
        public CADivisionalObjectiveDTO DivisionalObjective { get; set; }
        public CAPerspectiveDTO Perspective { get; set; }
        public List<KPIMeasureDTO> KPIMeasures { get; set; }
        public List<ParameterDTO> Parameters { get; set; }
        public List<AttachmentDTO> Attachments { get; set; }
        public OrgStructureDTO OrgStructure { get; set; }

        #endregion
        public bool CanCR { get; set; }

        public int OrgStructureID { get; set; }
        public decimal AccumulutiveTarget { get; set; }
        public decimal AccumulutiveValue { get; set; }

    }
}
