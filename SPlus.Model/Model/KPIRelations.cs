using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
   public class KPIRelations
    {
        public List<KPI> KPIs
        {
            get;
            set;
        }

        public decimal SAGIAWeight
        { 
            get; 
            set; 
        }
      
        public List<StrategicObjective> StrategicObjective
        { 
            get;
            set;
        }
        public List<General> Departments 
        { 
            get;
            set; 
        }

        //public List<KPI> KPIs 
        //{
        //    get; 
        //    set; 
        //}


    }
}
