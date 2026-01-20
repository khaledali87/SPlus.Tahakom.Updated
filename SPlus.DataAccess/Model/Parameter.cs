namespace SPlus.DataAccess.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Parameter
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public string ParameterName { get; set; }

        [Required]
        public decimal Value { get; set; }

        public string UpdateMethod { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int KPIID { get; set; }

        public string FieldID { get; set; }

        public string ParamId { get; set; }

        public string AggregationType { get; set; }

        [Required]
        public DateTime Modified { get; set; }

        [Required]
        public DateTime Created { get; set; }

        public virtual KPI KPI { get; set; }

    }
}
