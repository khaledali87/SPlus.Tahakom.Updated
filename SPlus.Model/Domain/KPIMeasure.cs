namespace SPlus.Model.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class KPIMeasure
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public decimal Target { get; set; }
        public decimal? Value { get; set; }
        public DateTime DueDate { get; set; }
        [Required]       
        public string Status { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int KPIID { get; set; }
        public DateTime Modified { get; set; }
        public DateTime Created { get; set; }
        public decimal? OutOfTarget { get; set; }
        public bool AllowUpdate { get; set; }
        [NotMapped]
        public bool IsActive { get; set; }
        public virtual KPI KPI { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ParameterValue> ParameterValues { get; set; }
        public decimal? AccumulutiveOutOfTarget { get; set; }
        public decimal? AccumulutiveValue { get; set; }
        public decimal? AccumulutiveTarget { get; set; }
        public string AccumulutiveStatus { get; set; }

        [NotMapped]
        public bool IsEditable { get; set; }
        public int CalculationMethod { get; set; }
    }
}
