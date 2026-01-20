using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class TaskCenterListDTO<T> where T : class
    {

        public int AllCount { get; set; }
        public int KPICount { get; set; }
        public int WorkProcedureCount { get; set; }
        public int MitigationActionsCount { get; set; }
        public T Data { get; set; }
    }
}
