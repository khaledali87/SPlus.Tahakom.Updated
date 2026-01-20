using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class MatrixDTO
    {
        public int ID { get; set; }

        public virtual ResourceDTO Resource { get; set; }
        public virtual RoleDTO Role { get; set; }
    }
}
