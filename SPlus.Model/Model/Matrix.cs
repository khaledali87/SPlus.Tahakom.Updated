using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class Matrix
    {
        private Role _Role = new Role();
        private Group _Group = new Group();
        private Resource _Resource = new Resource();
        public int ID { get; set; }
        public Role Role
        {
            get 
            {
                return _Role;
            }
            set
            {
                _Role = value;
            }
        }
        public Group Group
        {
            get
            {
                return _Group;
            }
            set
            {
                _Group = value;
            }
        }
        public Resource Resource
        {
            get
            {
                return _Resource;
            }
            set
            {
                _Resource = value;
            }
        }
    }
}
