using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class SubStrategicObjective
    {
        //private StrategicObjective _StrategicObjective = new StrategicObjective();
        private List<SubStrategicObjective> _AffectedStrategicObjective = new List<SubStrategicObjective>(); 
        private List<SubStrategicObjective> _EffectedStrategicObjective = new List<SubStrategicObjective>();
        private List<KPI> _KPI = new List<KPI>();
        private Perspective _Perspective = new Perspective();
        public int ID { get; set; }
        public string EnglishName { get; set; }
        public string ArabicName { get; set; }
        public string Sequence { get; set; }
        public int StrategicObjectiveID { get; set; }
        public int PerspectiveID { get; set; } 
        public string Code { get; set; }
        public bool DirectAccess { get; set; }
        public int Order { get; set; } 
        public Perspective Perspective 
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
        //public StrategicObjective StrategicObjective
        //{
        //    get
        //    {
        //        return _StrategicObjective;
        //    }
        //    set
        //    {
        //        _StrategicObjective = value;
        //    }
        //}
        public List<SubStrategicObjective> AffectedStrategicObjective
        {
            get
            {
                return _AffectedStrategicObjective;
            }
            set
            {
                _AffectedStrategicObjective = value;
            }
        }
        public List<SubStrategicObjective> EffectedStrategicObjective
        {
            get
            {
                return _EffectedStrategicObjective;
            }
            set
            {
                _EffectedStrategicObjective = value;
            }
        }
        public List<KPI> KPI
        {
            get
            {
                return _KPI;
            }
            set
            {
                _KPI = value;
            }
        }


    }
}
