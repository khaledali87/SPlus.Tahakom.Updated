using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class DashboardRequest
    {


        public List<string> Dashboards
        {
            get;
            set;
        }
        public string Type
        {
            get;
            set;
        }
        public int FunctionL1ID
        {
            get;
            set;
        }
        public int PillarID
        {
            get;
            set;
        }
        public int KPIID
        {
            get;
            set;
        }
    }
}
