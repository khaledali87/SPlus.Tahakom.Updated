using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPlus.Integration
{
    public class Database
    {
        public string DatabaseName { get; set; }
        public List<Schema> Schemas { get; set; }
    }
}
