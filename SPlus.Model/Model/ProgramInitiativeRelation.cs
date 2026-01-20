using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class ProgramInitiativeRelation
    {
        public int ID
        {
            set;
            get;
        }
        public int ProgramID
        {
            set;
            get;
        }

        public int InitiativeID
        {
            set;
            get;
        }
    }
}
