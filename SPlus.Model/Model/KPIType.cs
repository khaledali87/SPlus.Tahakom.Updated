using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    
    public class KPIType
    {
        private List<KPIStatus> _Statuses = new List<KPIStatus>();
        private Attachement _Attachement = new Attachement();

        public int ID { get; set; }
        public string Name { get; set; }
        public string EnglishName { get; set; }
        public string ArabicName { get; set; } 
        public string Icon { get; set; }
        public Attachement IconAttachement 
        {
            get
            {
                return _Attachement;
            }
            set
            {
                _Attachement = value;
            }
        }
        public int GracePeriod { get; set; }
        public List<KPIStatus> Statuses 
        {
            get
            {
                return _Statuses;
            }
            set
            {
                _Statuses = value;
            }
        }
    } 
    public class KPIStatus
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string EnglishName { get; set; }
        public string ArabicName { get; set; }
        public string Color { get; set; }
        public int Order { get; set; }
        public int? Min { get; set; }
        public int? Max { get; set; }
        public string Operator { get; set; }
        public string MinOperator { get; set; }
    } 
    public class KPIThreshold
    {
        public int ID { get; set; }
        public int Type { get; set; }
        public int StatusID { get; set; }
        public int? Min { get; set; }
        public int? Max { get; set; }
        public string Operator { get; set; }
        public string MinOperator { get; set; }
    }
}
