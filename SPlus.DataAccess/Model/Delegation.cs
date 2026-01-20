namespace SPlus.DataAccess.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Delegation")]
    public partial class Delegation
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [StringLength(255)]
        public string FromUser { get; set; }

        [StringLength(255)]
        public string ToUser { get; set; }
        
        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        [StringLength(255)]
        public string CreatedBy { get; set; }

        [StringLength(255)]
        public string ModifiedBy { get; set; }

        public DateTime? Created { get; set; }

        public DateTime? Modified { get; set; }

        public virtual User DelegatorUser { get; set; }
        public virtual User DelegatedUser { get; set; }
        public virtual User CreatedByModel { get; set; }
        public virtual User ModifiedByModel { get; set; }
    }
}
