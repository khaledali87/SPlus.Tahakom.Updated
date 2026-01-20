using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
   public class WeightAmounts
    {


        public List<KPI> KPIs
        {
            get;
            set;
        }

        public decimal GeneralWeight
        {
            get;
            set;
        }
        public List<FunctionL1> Departments
        {
            get;
            set;

        }
        public List<StrategicObjective> StrategicObjectives
        {
            get;
            set;

        }
       


    }
}
