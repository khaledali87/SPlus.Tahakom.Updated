using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class CreateKPIFormDTO : BaseLevelDTO
    {
        public bool IsDraft { get; set; }
        public int Type { get; set; }
        public int BaseWorkflow { get; set; }
        public int RequestID { get; set; }
        public string CreatedBy { get; set; }
        public UserListDTO CreatedByModel { get; set; }
        public DateTime Created { get; set; }
        public string EnglishDescription { get; set; }
        public string ArabicDescription { get; set; }
        public string EnglishEquation { get; set; }
        public string ArabicEquation { get; set; }
        public bool IsConstantTargets { get; set; }
        public string Polarity { get; set; }
        public string Source { get; set; }
        public string UnitOfMeasure { get; set; }
        public string EnglishUnitDetails { get; set; }
        public string ArabicUnitDetails { get; set; }
        public int CalculationMethod { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime BaselineDate { get; set; }
        public int Years { get; set; }
        public string Frequency { get; set; }
        public string DataSource { get; set; }
        public string Formula { get; set; }
        public decimal Baseline { get; set; }
        public string Code { get; set; }
        public string KPISource { get; set; }

        #region Relation Models
        public SingularKPITypeDTO KPIType { get; set; }
        public UserListDTO ChampionModel { get; set; }
        public UserListDTO OwnerModel { get; set; }
        public CAPerspectiveDTO Perspective { get; set; }
        public CAStrategicObjectiveDTO StrategicObjective { get; set; }
        public CADivisionalObjectiveDTO DivisionalObjective { get; set; }
        public CAStrategyDTO Strategy { get; set; }
        public List<KPIMeasureDTO> KPIMeasures { get; set; }
        public List<KPICommentDTO> KPIComments { get; set; }
        public List<AttachmentDTO> Attachments { get; set; }
        public List<ParameterDTO> Parameters { get; set; }
        public OrgStructureDTO OrgStructure { get; set; }
        #endregion
    }
}
