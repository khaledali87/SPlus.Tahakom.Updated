using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class ParameterDTO
    {
        public int ID { get; set; }

        public string ParameterName { get; set; }

        public decimal Value { get; set; }

        public string UpdateMethod { get; set; }

        public string Description { get; set; }

        public int KPIID { get; set; }

        public string FieldID { get; set; }

        public string ParamId { get; set; }

        public string AggregationType { get; set; }

        public DateTime Modified { get; set; }

        public DateTime Created { get; set; }

        public bool IsConstant { get; set; }
        public decimal ConstantValue { get; set; }
    }
}
