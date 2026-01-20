using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class Permission
    {
        private Role _Roles = new Role();
        private Resource _Resources = new Resource();
        public Role Role
        {
            get
            {
                return _Roles;
            }
            set
            {
                _Roles = value;
            }
        }
        public Resource Resource
        {
            get
            {
                return _Resources;
            }
            set
            {
                _Resources = value;
            }
        }
    }
}
