using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPlus.Model.Common;

namespace SPlus.Model
{
    public class UserRelation
    {
        public int ID
        {
            set;
            get;
        }
        public int UserID
        {
            set;
            get;
        }

        public int GroupID
        {
            set;
            get;
        }
    }
}
