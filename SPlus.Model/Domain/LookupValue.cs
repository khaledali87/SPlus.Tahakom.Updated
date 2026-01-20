namespace SPlus.Model.Domain
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
        public string English { get; set; }
        public string Arabic { get; set; } 
        public int? LookupID { get; set; }
        public string Order0 { get; set; }
        public string Color { get; set; }
        public string Value { get; set; }
        public string Others { get; set; }
        public string Description { get; set; }
        public DateTime? Modified { get; set; }
        public DateTime? Created { get; set; }
        public virtual Lookup Lookup { get; set; }

    }
}
