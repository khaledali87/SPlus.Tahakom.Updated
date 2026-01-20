using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class Handshake
    {

        public int ID
        {
            set;
            get;
        }

        public string Name
        {
            set;
            get;
        }

        public List<cLookup> Items
        {
            set;
            get;
        }
    }
}
