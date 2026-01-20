namespace SPlus.DataAccess.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LookupValue")]
    public partial class LookupValue
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [StringLength(255)]
        public string English { get; set; }

        [StringLength(255)]
        public string Arabic { get; set; } 

        public int? LookupID { get; set; }

        [StringLength(255)]
        public string Order0 { get; set; }

        [StringLength(255)]
        public string Color { get; set; }

        [StringLength(255)]
        public string Value { get; set; }

        [StringLength(255)]
        public string Others { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        public DateTime? Modified { get; set; }

        public DateTime? Created { get; set; }
        
        public virtual Lookup Lookup { get; set; }

    }
}
