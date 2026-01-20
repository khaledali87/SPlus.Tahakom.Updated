using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class GroupListDTO
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsDeletable { get; set; }
        public bool IsEditable { get; set; }
        public bool IsReadOnly { get; set; }
        public int UsersCount { get; set; }
    }
}
