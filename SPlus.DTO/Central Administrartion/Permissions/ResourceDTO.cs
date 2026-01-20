using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class ResourceDTO
    {
        public int ID { get; set; }

        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsScreen { get; set; }

        public string PropertyName { get; set; }

        public int PropertyValue { get; set; }

        public int LevelID { get; set; }
    }
    public class RoleDTO
    {
        public int ID { get; set; }
        public string Role { get; set; }
    }

}
