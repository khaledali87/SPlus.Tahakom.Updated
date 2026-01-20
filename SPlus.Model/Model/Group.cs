using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class Group
    {
        private List<Permission> _Permissions = new List<Permission>();
        private List<UserInfo> _UserInfo = new List<UserInfo>();
        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsDeletable { get; set; }
        public bool IsEditable { get; set; }
        public int UserCount { get; set; } 
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
        public List<Permission> Permissions
        {
            get
            {
                return _Permissions;
            }
            set
            {
                _Permissions = value;
            }
        }
    }
}
