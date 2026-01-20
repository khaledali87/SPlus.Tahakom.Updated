using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncJobs.Models
{

    public class KPIData
    {
        public int ID
        {
            get;
            set;
        }
        public int StrategicObjectiveID
        {
            get;
            set;
        }
        public string Polarity
        {
            get;
            set;
        }
        public string Direction
        {
            get;
            set;
        }
        public string Status
        {
            get;
            set;
        }
        public decimal TempTarget
        {
            get;
            set;
        }
        public decimal TempValue
        {
            get;
            set;
        }
        public decimal Value
        {
            get;
            set;
        }
        public decimal Target
        {
            get;
            set;
        }
        public decimal SAGIAWeight
        {
            get;
            set;
        }
        public decimal DepartmentWeight
        {
            get;
            set;
        }
        public decimal StrategicObjectiveWeight
        {
            get;
            set;
        }
        public decimal OutOfTarget
        {
            get;
            set;
        }
        public decimal Performance
        {
            get;
            set;
        }
        public decimal SAGIAPerformance
        {
            get;
            set;
        }
        public decimal DepartmentPerformance
        {
            get;
            set;
        }

        //As 7/10/2019
        public int DepartmentId
        {
            get;
            set;
        }

        //As 16/10/2019
        public string KPIType { get; set; }
        public decimal Baseline
        {
            get;
            set;
        }
        
    }
}
