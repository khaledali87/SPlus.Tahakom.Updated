using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class UpdateKPIDTO : BaseDTO
    {
        public string ReferenceNumber { get; set; }
        public string EnglishDescription { get; set; }
        public string ArabicDescription { get; set; }
        public string EnglishEquation { get; set; }
        public string ArabicEquation { get; set; }
        public string EnglishUnitDetails { get; set; }
        public string ArabicUnitDetails { get; set; }
        public string UnitOfMeasure { get; set; }
        public OrgStructureDTO Division { get; set; }
        public OrgStructureDTO OrgStructure { get; set; }
        public StrategicObjectiveListDTO StrategicObjective { get; set; }
        public CADivisionalObjectiveDTO DivisionalObjective { get; set; }
        public string Formula { get; set; }
        public string DataSource { get; set; }
        public DateTime BaselineDate { get; set; }
        public int Years { get; set; }
        public UserDTO ChampionModel { get; set; }
        public UserDTO OwnerModel { get; set; }
        public UserDTO SponsorModel { get; set; }
        public List<ParameterDTO> Parameters { get; set; }
        public List<KPIMeasureDTO> KPIMeasures { get; set; }
        public List<AttachmentDTO> Attachments { get; set; }
        public SingularKPITypeDTO KPIType { get; set; }

    }
}
