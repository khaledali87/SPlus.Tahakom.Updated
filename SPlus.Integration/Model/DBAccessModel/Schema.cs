using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPlus.Integration
{ 
    public class Schema
    {
        public string SchemaId { get; set; }
        public string SchemaName { get; set; }
        public List<Table> Tables { get; set; }
    }
}
