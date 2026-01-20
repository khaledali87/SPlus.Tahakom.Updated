using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class OperationalStrategyDTO : BaseDTO
    {
        public string EnglishVision { get; set; }
        public string ArabicVision { get; set; }
        public string EnglishMission { get; set; }
        public string ArabicMission { get; set; }
        public AttachmentDTO Attachment { get; set; } = null;
        public List<ValueDTO> Values { get; set; }
        public List<OperationalStrategyDivisionDTO> Divisions { get; set; }
    }
}
