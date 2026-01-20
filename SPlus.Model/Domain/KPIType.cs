namespace SPlus.Model.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("KPIType")]
    public partial class KPIType
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Key]
        public int KPITypeID { get; set; }

        [Required]
        public string EnglishName { get; set; }

        [Required]
        public string ArabicName { get; set; }

        public bool IsDepartmental { get; set; }

        [NotMapped]
        public bool IsDeletable { get; set; }
        public int GracePeriod { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<KPIThreshold> KPIThresholds { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Workflow> Workflows { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<KPI> KPIs { get; set; }
        public virtual ReminderConfiguration ReminderConfiguration { get; set; }
        [NotMapped]
        public Attachment Attachment { get; set; }
    }
}
