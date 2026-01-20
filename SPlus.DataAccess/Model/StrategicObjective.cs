namespace SPlus.DataAccess.Model
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
        [StringLength(255)]
        public string EnglishName { get; set; }

        [Required]
        [StringLength(255)]
        public string ArabicName { get; set; }

        public int PerspectiveID { get; set; }

        public DateTime Modified { get; set; }

        public DateTime Created { get; set; }
        public decimal Weight { get; set; }

        public int? ThemeID { get; set; }

        public virtual Perspective Perspective { get; set; }

        public virtual Theme Theme { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<KPI> KPIs { get; set; }
    }
}