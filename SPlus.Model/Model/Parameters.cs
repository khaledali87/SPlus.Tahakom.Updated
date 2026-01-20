using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class Parameters
    {
        public string ParamId { get; set; }
        public string ParameterName { get; set; }
        public string UpdateMethod { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        
        public string KPIId { get; set; }
        public int Id { get; set; }

        public string FieldId { get; set; }
        public string AggregationType { get; set; }
        public string TableId { get; set; }


    }
}
