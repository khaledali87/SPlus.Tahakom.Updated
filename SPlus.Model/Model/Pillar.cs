using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class Pillar
    {
        private List<StrategicObjective> _StrategicObjectives = new List<StrategicObjective>();
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
        public int Workstream 
        {  
            get; 
            set; 
        }
        public string Icon 
        { 
            get; 
            set; 
        }
        public Attachement IconAttachement
        { 
            get;
            set;
        }
        public string Color
        {
            get;
            set;
        }
        public string Code
        {
            get;
            set;
        }
        public string Description 
        { 
            get;
            set; 
        }
        public string DescriptionArabic
        {
            get;
            set;
        }
        public int Order
        {
            get;
            set;
        }
        public string Type
        {
            set;
            get;
        }
        public List<StrategicObjective> StrategicObjectives
        {
            get
            {
                return _StrategicObjectives;
            }
            set
            {
                _StrategicObjectives = value;
            }
        }
    }
}
