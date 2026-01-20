using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class Dashboard
    {
        private UpdatedStatus _UpdatedStatus = new UpdatedStatus();
        private Status _Status = new Status();
        private List<UpdatedStatusDepartment> _UpdatedStatusDepartments = new List<UpdatedStatusDepartment>();
        private List<UpdatedStatusChampion> _UpdatedStatusChampions = new List<UpdatedStatusChampion>();
        private List<KPI> _KPIs = new List<KPI>();
        private List<PillarsPerformance> _PillarsPerformance = new List<PillarsPerformance>();
        private GAZTPerformance _GAZTPerformance= new GAZTPerformance();
        private List<StrategicObjectivePerformance> _StrategicObjectivePerformances = new List<StrategicObjectivePerformance>();

        
        public UpdatedStatus UpdatedStatus
        {
            get
            {
                return _UpdatedStatus;
            }
            set
            {
                _UpdatedStatus = value;
            }
        }
        public Status Status
        {
            get
            {
                return _Status;
            }
            set
            {
                _Status = value;
            }
        }
        public List<UpdatedStatusDepartment> UpdatedStatusDepartments
        {
            get
            {
                return _UpdatedStatusDepartments;
            }
            set
            {
                _UpdatedStatusDepartments = value;
            }
        }

        public List<UpdatedStatusChampion> UpdatedStatusChampions
        {
            get
            {
                return _UpdatedStatusChampions;
            }
            set
            {
                _UpdatedStatusChampions = value;
            }
        }

        public List<KPI> KPIs
        {
            get
            {
                return _KPIs;
            }
            set
            {
                _KPIs = value;
            }
        }
        public List<PillarsPerformance> PillarsPerformance
        {
            get
            {
                return _PillarsPerformance;
            }
            set
            {
                _PillarsPerformance = value;
            }
        }

        public GAZTPerformance GAZTPerformance
        {
            get
            {
                return _GAZTPerformance;
            }
            set
            {
                _GAZTPerformance = value;
            }
        }

        public List<StrategicObjectivePerformance> StrategicObjectivePerformance
        {
            get
            {
                return _StrategicObjectivePerformances;
            }
            set
            {
                _StrategicObjectivePerformances = value;
            }
        }

        public object Analytical
        {
            get;
            set;
        }
    }
}
