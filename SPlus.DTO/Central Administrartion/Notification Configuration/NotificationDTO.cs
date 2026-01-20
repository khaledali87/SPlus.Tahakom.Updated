using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class NotificationDTO : BaseDTO
    {

        public string AssignedTo { get; set; }

        public string Status { get; set; }

        public int RelatedItemID { get; set; }
        public string ItemType { get; set; }
        public string NotificationType { get; set; }

        public DateTime Modified { get; set; }

        public DateTime Created { get; set; }
    }
}
