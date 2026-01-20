using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class CAStrategyDTO : BaseDTO
    {
        public bool IsSloganEnabled { get; set; }
        public string EnglishSlogan { get; set; }
        public string ArabicSlogan { get; set; }
        public bool IsVisionEnabled { get; set; }
        public string EnglishVision { get; set; }
        public string ArabicVision { get; set; }
        public bool IsMissionEnabled { get; set; }
        public string EnglishMission { get; set; }
        public string ArabicMission { get; set; }
        public List<string> Years { get; set; }
        public AttachmentDTO Attachment { get; set; }
    }
}
