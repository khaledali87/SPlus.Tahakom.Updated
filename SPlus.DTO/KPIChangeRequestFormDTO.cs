using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class KPIChangeRequestFormDTO : BaseLevelDTO
    {
        public bool IsDraft { get; set; }
        public int Type { get; set; }
        public int BaseWorkflow { get; set; }
        public int RequestID { get; set; }
        public List<int> ChangeType { get; set; }
        public string CreatedBy { get; set; }
        public UserListDTO CreatedByModel { get; set; }
        public DateTime Created { get; set; }
        public string EnglishDescription { get; set; }
        public string ArabicDescription { get; set; }
        public string EnglishEquation { get; set; }
        public string ArabicEquation { get; set; }
        public decimal Baseline { get; set; }
        public UserListDTO ChampionModel { get; set; }
        public UserListDTO OwnerModel { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; }
        public int Years { get; set; }
        public List<KPIMeasureDTO> KPIMeasures { get; set; }

        public List<WeightDTO> KPIsWeight { get; set; }

        public int CalculationMethod { get; set; }

        public KPIDetailsDTO OldKPI { get; set; }

       
        public List<CAKPIWeightDTO> OldKPIWeight { get; set; }
        public List<AttachmentDTO> Attachments { get; set; }

    }
}
