using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class StrategicMapCard
    {
        private List<Perspective> _Perspective = new List<Perspective>();
        //private List<Pillar> _Pillar = new List<Pillar>();
        private List<Goals> _Goals = new List<Goals>();
        public List<Perspective> Perspective
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
        //public List<Pillar> Pillar
        //{
        //    get
        //    {
        //        return _Pillar;
        //    }
        //    set
        //    {
        //        _Pillar = value;
        //    }
        //}
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
