using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class FunctionL2
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
        public List<FunctionL3> FunctionsL3
        {
            set;
            get;
        }
    }
}
