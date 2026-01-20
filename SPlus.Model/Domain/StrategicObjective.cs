namespace SPlus.Model.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("StrategicObjective")]
    public partial class StrategicObjective
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public string EnglishName { get; set; }

        [Required]
        public string ArabicName { get; set; }
       
        public string ArabicDescription { get; set; }
        
        public string EnglishDescription { get; set; }

        [Required]        
        public int Order { get; set; }

        [Required]
       
        public string Code { get; set; }

        [NotMapped]
        public bool IsDeletable { get; set; }

        public DateTime Modified { get; set; }

        public DateTime Created { get; set; }

        public decimal Weight { get; set; }

        public int ThemeID { get; set; }

        public virtual Theme Theme { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<KPI> KPIs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DivisionalObjective> DivisionalObjectives { get; set; }
        [NotMapped]
        public decimal Performance { get; set; }
        [NotMapped]
        public string Status { get; set; }
    }
}