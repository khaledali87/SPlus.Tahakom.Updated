using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class Login
    {
        private List<Permission> _Permissions = new List<Permission>();
        private List<UserInfo> _DelegatedData = new List<UserInfo>();
        public UserInfo UserData { set; get; }
        public string Token { set; get; }
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
        public List<UserInfo> DelegatedData
        {
            get
            {
                return _DelegatedData;
            }
            set
            {
                _DelegatedData = value;
            }
        }

    }
}
