using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{   
    public class KPIViewModel
    {
        private General _GeneralData = new General();
        private UserInfo _UserInfo = new UserInfo();

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

       

        public int StrategicObjectiveID
        {
            set;
            get;
        }

        public string Baseline
        {
            set;
            get;
        }

        public int Target
        {
            set;
            get;
        }


        public UserInfo KPIOwner1
        {
            get
            {
                return _UserInfo;
            }
            set
            {
                _UserInfo = value;
            }
        }

        public UserInfo KPIOwner2
        {
            get
            {
                return _UserInfo;
            }
            set
            {
                _UserInfo = value;
            }
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

        public int CurrentValue
        {
            set;
            get;
        }

        public bool Status
        {
            set;
            get;
        }
        public string Direction
        {
            set;
            get;
        }

        public string LastUpdate
        {
            set;
            get;
        }

        public List<KPIMeasure> Measures
        {
            set;
            get;
        }
    }
}
