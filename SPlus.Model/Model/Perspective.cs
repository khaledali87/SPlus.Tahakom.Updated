using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class Perspective
    {
        private List<SubStrategicObjective> _SubStrategicObjectives = new List<SubStrategicObjective>();
        public int ID 
        { 
            get;
            set;
        }
        public string EnglishName 
        { 
            get;
            set; 
        }
        public string ArabicName 
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
        public List<SubStrategicObjective> SubStrategicObjectives
        {
            get
            {
                return _SubStrategicObjectives;
            }
            set
            {
                _SubStrategicObjectives = value;
            }
        }
    }
}
