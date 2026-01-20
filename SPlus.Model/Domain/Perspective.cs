namespace SPlus.Model.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Perspective")]
    public partial class Perspective
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Perspective()
        {
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]       
        public string EnglishName { get; set; }

        [Required]       
        public string ArabicName { get; set; }

        [Required]        
        public int Order { get; set; }

        [Required]
        public int StrategyID { get; set; }
        public virtual Strategy Strategy { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<KPI> KPIs { get; set; }


        [NotMapped]
        public bool IsDeletable { get; set; } = false;
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
        [NotMapped]
        public Attachment Attachment { get; set; }

        public string BackgroundColor { get; set; }


    }
}
