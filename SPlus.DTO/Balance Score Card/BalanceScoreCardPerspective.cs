using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class BalanceScoreCardPerspective : BaseDTO
    {
        public AttachmentDTO Attachment { get; set; }

        public string Color { get; set; }
    }
}
