using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class WorkflowStep
    {
        public int ID { get; set; }
        public string EnglishName { get; set; }
        public string ArabicName { get; set; }
        public string Approver { get; set; }
        public string WorkflowID { get; set; }
        public bool IsGroup { get; set; }
        public int Order { get; set; }
        public string Status { get; set; }

        public bool isDeleted { get; set; }

    }
}
