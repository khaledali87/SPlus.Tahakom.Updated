using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class FunctionL1
    {
        public int ID
        {
            set;
            get;
        }
        public string EnglishName
        {
            set;
            get;
        }

        public string ArabicName
        {
            set;
            get;
        }

        public List<FunctionL2> FunctionsL2
        {
            set;
            get;
        }

        public decimal DepartmentWeight
        {
            get;
            set;
        }
        public decimal Performance
        {
            get;
            set;
        }
        public List<KPI> KPIs
        {
            get;
            set;
        }
    }
}
