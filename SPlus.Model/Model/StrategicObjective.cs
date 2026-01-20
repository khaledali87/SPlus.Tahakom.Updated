using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class StrategicObjective
    {
        public int ID { get; set; }
        public string EnglishName { get; set; } 
        public string ArabicName { get; set; }
        public int PillarID { get; set; }
        public string SequenceNo { get; set; }
        public string Code { get; set; }


        //private Pillar _Pillars = new Pillar();


        //public Pillar Pillars
        //{
        //    get
        //    {
        //        return _Pillars;
        //    }
        //    set
        //    {
        //        _Pillars = value;
        //    }
        //}


        private List<SubStrategicObjective> _SubStrategicObjective  = new List<SubStrategicObjective>();

        public List<SubStrategicObjective> SubStrategicObjective
        {
            get
            {
                return _SubStrategicObjective;
            }
            set
            {
                _SubStrategicObjective = value;
            }
        }

        private List<Lookup> _Pillar = new List<Lookup>();
        private List<Lookup> _Theme = new List<Lookup>();
        //private List<Theme> _Theme1 = new List<Theme>();
        private General _GeneralData = new General();
        private List<KPI> _KPIs = new List<KPI>();

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
        public decimal Progress
        {
            get;
            set;
        }
        public decimal Weight
        {
            set;
            get;
        }

        public decimal StartegicObjectiveWeight
        {
            get;
            set;
        }
        public List<Lookup> Theme
        {
            get
            {
                return _Theme;
            }
            set
            {
                _Theme = value;
            }
        }


        public List<Lookup> Pillar
        {
            get
            {
                return _Pillar;
            }
            set
            {
                _Pillar = value;
            }
        }

        public List<Benefit> Benefits { set; get; }
        public decimal BenefitWeight { get; set; }

       
        public decimal Performance { get; set; } 
        public decimal BenefitsPerformance { get; set; }
    }
}
