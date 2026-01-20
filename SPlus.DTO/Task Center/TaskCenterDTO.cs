using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class TaskCenterDTO<T> where T : class
    {
        //public List<ApprovalDTO> Approvals { get; set; }
        //public List<UpdateListDTO> Updates { get; set; }

        public int AllCount { get; set; }
        public int KPICount { get; set; }
        public int WorkProcedureCount { get; set; }
        public int MitigationActionsCount { get; set; }
        public T Data { get; set; }
    }
}
