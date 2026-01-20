using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncJobs.Models
{
    public class Group
    {
        private List<UserInfo> _UserInfo = new List<UserInfo>();
        public int ID { get; set; }
        public string Title { get; set; }
        public List<UserInfo> Users
        {
            get
            {
                return _UserInfo;
            }
            set
            {
                _UserInfo = value;
            }
        }
    }
}
