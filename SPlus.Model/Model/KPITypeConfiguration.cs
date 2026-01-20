using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class KPITypeConfiguration
    {
        private KPIType _KPIType = new KPIType();
        private List<StatusThreshold> _StatusThreshold = new List<StatusThreshold>();
        private List<UpdateWorkflow> _UpdateWorkflow = new List<UpdateWorkflow>();
        private Reminder _Reminder = new Reminder();

        public KPIType KPIType
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
        public List<StatusThreshold> StatusThreshold
        {
            get
            {
                return _StatusThreshold;
            }
            set
            {
                _StatusThreshold = value;
            }
        }
        public List<UpdateWorkflow> UpdateWorkflow 
        {
            get
            {
                return _UpdateWorkflow;
            }
            set
            {
                _UpdateWorkflow = value;
            }
        }
        public Reminder Reminder
        {
            get
            {
                return _Reminder;
            }
            set
            {
                _Reminder = value;
            }
        }


    }
    public class Reminder
    {
        public int ID { get; set; }
        public int Before { get; set; }
        public int FirstReminder { get; set; }
        public int SecondReminder { get; set; }
        public int TypeID { get; set; }
    } 
    public class StatusThreshold
    {
        public int Type { get; set; }
        public int StatusID { get; set; }
        public string EnglishName { get; set; }
        public string ArabicName { get; set; }
        public int? Min { get; set; }
        public int? Max { get; set; } 
        public string Color { get; set; }
        public string Operator { get; set; }
        public string MinOperator { get; set; }
    }
    public class UpdateWorkflow
    {
        public int StepID { get; set; }
        public string KPIType { get; set; }
        public string EnglishTitle { get; set; }
        public string ArabicTitle { get; set; }
        public string Role { get; set; }
        public int SchemeID { get; set; }
        public bool IsGroup { get; set; }
        public int Order { get; set; } 
        public bool isDeleted { get; set; }
    } 
}
