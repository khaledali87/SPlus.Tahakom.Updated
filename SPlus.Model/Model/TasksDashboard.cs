using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class TasksDashboard
    {
        public TasksDashboard()
        {
            TotalTasks = 0;
            PendingTasks = 0;
            ApprovedTasks = 0;
            RejectedTasks = 0;
            TotalKPIs = 0;
            RequireUpdateKPIs = 0;
            UptoDateKPIs = 0;
            OverDueKPIs = 0;
        }
        public int TotalTasks { get; set; }
        public int PendingTasks { get; set; }
        public int ApprovedTasks { get; set; }
        public int RejectedTasks { get; set; }
        public int TotalKPIs { get; set; }
        public int RequireUpdateKPIs { get; set; }
        public int UptoDateKPIs { get; set; }
        public int OverDueKPIs { get; set; }
        public DateTime? OlderDueDate { get; set; }

    }
}
