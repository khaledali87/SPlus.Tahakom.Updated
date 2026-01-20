using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class Department
    {
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
        public int ParentID
        {
            set;
            get;
        }
    }
}
