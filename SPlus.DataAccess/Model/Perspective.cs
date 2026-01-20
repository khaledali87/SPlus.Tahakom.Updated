namespace SPlus.DataAccess.Model
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
            StrategicObjectives = new HashSet<StrategicObjective>();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [StringLength(255)]
        public string EnglishName { get; set; }

        [Required]
        [StringLength(255)]
        public string ArabicName { get; set; }

        [StringLength(255)]
        public string Icon { get; set; }
        public string Color { get; set; }

        public decimal Weight { get; set; }
        public DateTime? Created { get; set; }

        public DateTime? Modified { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StrategicObjective> StrategicObjectives { get; set; }
        [NotMapped]
        public Attachment Attachment { get; set; } = new Attachment(); 
    }
}
