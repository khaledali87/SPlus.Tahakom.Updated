using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPlus.Integration
{
    public class Table
    {
        public string TableId { get; set; }
        public string TableName { get; set; }
        public List<Column> Columns { get; set; }
    }
}
