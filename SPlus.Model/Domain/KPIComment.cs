namespace SPlus.Model.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class KPIComment
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int KPIID { get; set; }

        [Required]
        public string Comment { get; set; }

        [Required]
        public string CreatedBy { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public virtual KPI KPI { get; set; }
        [NotMapped]
        public User CreatedByModel { get; set; }
    }
}
