using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;


namespace SPlus.Model.Domain
{
    public class KPIHistory
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        public int KPIID { get; set; }
        [Required]
        public decimal Baseline { get; set; }
    
        public decimal Weight { get; set; }
        public decimal BusinessUnitWeight { get; set; }

        public string Status { get; set; }
    
        public decimal OutOfTarget { get; set; }
        public decimal AccumulutiveOutOfTarget { get; set; }
       
        public decimal Target { get; set; }
    
        public decimal Value { get; set; }

        public DateTime HistoryDate { get; set; }


        public decimal SumCumulativePerformance { get; set; }
        public decimal AverageCumulativePerformance { get; set; }
    }
}
