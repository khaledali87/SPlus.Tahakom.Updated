using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class Division
    {
        private List<Division> _Children = new List<Division>();
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
        public int? ParentID
        {
            set;
            get;
        }
        public List<Division> Children
        {
            get;
            //{
            //    return _Children;
            //}
            set;
            //{
            //    _Children = value;
            //}
        }

    }
}
