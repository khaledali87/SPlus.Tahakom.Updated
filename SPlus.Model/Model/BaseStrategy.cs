using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class BaseStrategy
    {
        private List<Workstream> _Workstreams = new List<Workstream>();
        private List<Goals> _Goals = new List<Goals>();

        public List<Workstream> Workstreams
        {
            get
            {
                return _Workstreams;
            }
            set
            {
                _Workstreams = value;
            }
        }
        public List<Goals> Goals
        {
            get
            {
                return _Goals;
            }
            set
            {
                _Goals = value;
            }
        }
    }
   
}
