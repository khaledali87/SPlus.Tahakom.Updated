using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class KPIMeasureDTO
    {
        public int ID { get; set; }
        public decimal Value { get; set; }
        public decimal Target { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime Modified { get; set; }
        public DateTime Created { get; set; }
        public decimal OutOfTarget { get; set; }
        public bool AllowUpdate { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
        public decimal? AccumulutiveOutOfTarget { get; set; }
        public decimal? AccumulutiveValue { get; set; }
        public decimal? AccumulutiveTarget { get; set; }
        public string AccumulutiveStatus { get; set; }
        public bool IsEditable { get; set; }
        public int CalculationMethod { get; set; }
        
    }
}
