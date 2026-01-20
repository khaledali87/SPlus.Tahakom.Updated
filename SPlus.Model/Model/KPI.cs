using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class KPI
    {
        private StrategicObjective _StrategicObjective = new StrategicObjective();
        private General _GeneralData = new General();
        private UserInfo _UserInfo1 = new UserInfo();
        private UserInfo _UserInfo2 = new UserInfo();
        private UserInfo _UserInfo3 = new UserInfo();
        private Attachement _Attachement = new Attachement();
        private List<Attachement> _Attachements = new List<Attachement>();
        private General _FunctionL1 = new General();
        private General _FunctionL2 = new General();
        private General _FunctionL3 = new General();
        private KPIType _KPIType = new KPIType();
        private DivisionDetail _Division = new DivisionDetail();
        private DepartmentDetail _Department = new DepartmentDetail();
        private PerspectiveDetail _Perspective = new PerspectiveDetail();
        private SubStrategicObjectiveDetails _SubStrategicObjectiveDetails = new SubStrategicObjectiveDetails();
        public string Code 
        {
            set;
            get;
        } 
        public string ReferenceNo { get; set; }
        public int SubStrategicObjectiveID { get; set; }
        public General GeneralData
        {
            get
            {
                return _GeneralData;
            }
            set
            {
                _GeneralData = value;
            }
        }

        public string Baseline
        {
            set;
            get;
        }

        public decimal Target
        {
            set;
            get;
        }

        public string Frequency
        {
            set;
            get;
        }

        public string UnitOfMeasure
        {
            set;
            get;
        }

        public string EnglishDescription
        {
            set;
            get;
        }

        public string ArabicDescription
        {
            set;
            get;
        }

        public string EnglishEquation
        {
            set;
            get;
        }

        public string ArabicEquation
        {
            set;
            get;
        }
        public string KPIType { get; set; }
        public KPIType KPITypes
        {
            get
            {
                return _KPIType;
            }
            set
            {
                _KPIType = value; 
            }
        }
        public string Tier 
        {
            set;
            get;
        }
        public string Goal
        {
            set;
            get;
        }
        public bool IsBoard
        {
            set;
            get;
        }

        public UserInfo Owner
        {
            get
            {
                return _UserInfo1;
            }
            set
            {
                _UserInfo1 = value;
            }
        }

        public UserInfo Champion
        {
            get
            {
                return _UserInfo2;
            }
            set
            {
                _UserInfo2 = value;
            }
        }
        public UserInfo Sponsor 
        {
            get
            {
                return _UserInfo3;
            }
            set
            {
                _UserInfo3 = value;
            }
        }

        public string Status
        {
            set;
            get;
        }

        public string Direction
        {
            set;
            get;
        }

        public string TempStatus
        {
            set;
            get;
        }

        public string TempDirection
        {
            set;
            get;
        }

        public decimal Value
        {
            set;
            get;
        }

        public decimal TempValue
        {
            set;
            get;
        }
        public decimal TempTarget
        {
            set;
            get;
        }

        public string DataSource
        {
            set;
            get;
        }
        public string FocusArea
        {
            set;
            get;
        }

        public int Years
        {
            set;
            get;
        }

        public string BaselineYear
        {
            set;
            get;
        }
        public DateTime BaselineDate 
        {
            set;
            get;
        }
        public string Duration
        {
            set; 
            get;
        }

        public string Polarity
        {
            set;
            get;
        }

        public string UnitDetails
        {
            set;
            get;
        } 
        public string UnitDetailsAR
        {
            set;
            get;
        }
        public string Unit
        {
            set;
            get;
        }

        public General FunctionL1
        {
            get
            {
                return _FunctionL1;
            }
            set
            {
                _FunctionL1 = value;
            }
        }

        public General FunctionL2
        {
            get
            {
                return _FunctionL2;
            }
            set
            {
                _FunctionL2 = value;
            }
        }

        public General FunctionL3
        {
            get
            {
                return _FunctionL3;
            }
            set
            {
                _FunctionL3 = value;
            }
        }

        public List<KPIMeasure> Measures
        {
            set;
            get;
        }

        public List<Attachement> Attachements
        {
            get
            {
                return _Attachements;
            }
            set
            {
                _Attachements = value;
            }
        }

        public string StartDate
        {
            set;
            get;
        }

        public string RequireUpdate
        {
            get;
            set;
        }
        public StrategicObjective StrategicObjective
        {
            get
            {
                return _StrategicObjective;
            }
            set
            {
                _StrategicObjective = value;
            }
        }
        public decimal GeneralWeight
        {
            set;
            get;
        }
        public decimal StrategicObjectiveWeight
        {
            set;
            get;
        }
        public decimal DepartmentWeight
        {
            set;
            get;
        }
        public decimal OutOfTarget
        {
            set;
            get;
        }
        public string Note
        {
            set;
            get;
        }
        public bool EnableEscalation
        {
            set;
            get;
        }
        public decimal SAGIAPerformance
        {
            set;
            get;
        }
        public string DepartmentPerformance { get; set; }
        public string CreationDate { get; set; }
        public int Department { get; set; }
        public int Division { get; set; }
        public bool? IsLocked { get; set; }
        public bool AllowLock { get; set; }
        public DateTime? UnlockDate { get; set; }
        //As Parameters
        public string ParameterFormula { get; set; }
        public List<Parameters> Parameters
        {
            set;
            get;
        }

        //End AS
        public SubStrategicObjectiveDetails SubStrategicObjectiveDetails
        {
            get
            {
                return _SubStrategicObjectiveDetails;
            }
            set
            {
                _SubStrategicObjectiveDetails = value;
            }
        }

        //As KPI Editable
        public bool IsEditable { set; get; }
        public int RequestID { set; get; } 
        public bool DirectAccess { get; set; }

        //End  AS
        public DepartmentDetail DepartmentDetail
        {
            get
            {
                return _Department;
            }
            set
            { 
                _Department = value;
            }
            
        }
        public PerspectiveDetail PerspectiveDetail
        {
            get
            {
                return _Perspective;
            }
            set
            {
                _Perspective = value;
            }
        }
        public DivisionDetail DivisionDetail
        {
            get
            { 
                return _Division;
            }
            set
            {
                _Division = value;
            }
        }
    }
    public class SubStrategicObjectiveDetails
    {
        public int ID { get; set; }
        public string EnglishName { get; set; } 
        public string ArabicName { get; set; }
    }
    public class PerspectiveDetail
    {
        public int ID { get; set; }
        public string EnglishName { get; set; }
        public string ArabicName { get; set; }
    }
    public class DivisionDetail
    {
        public int ID { get; set; }
        public string EnglishName { get; set; }
        public string ArabicName { get; set; }
    }
    public class DepartmentDetail
    {
        public int ID { get; set; }
        public string EnglishName { get; set; }
        public string ArabicName { get; set; }
    }
}
