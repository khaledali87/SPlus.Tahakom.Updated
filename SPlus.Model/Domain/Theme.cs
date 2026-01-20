namespace SPlus.Model.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Theme")]
    public partial class Theme
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public string EnglishName { get; set; }

        [Required]
        public string ArabicName { get; set; }

        [Required]
        public int Order { get; set; }

        [Required]
        public decimal Weight { get; set; }

        [Required]
        public string BackgroundColor { get; set; }

        [Required]
        public string FrameColor { get; set; }

        [Required]
        public DateTime Modified { get; set; }

        [Required]
        public DateTime Created { get; set; }  
        
        public int StrategyID { get; set; }

        public virtual Strategy Strategy { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StrategicObjective> StrategicObjectives { get; set; }

        [NotMapped]
        public Attachment Attachment { get; set; }

        [NotMapped]
        public bool IsDeletable { get; set; }
    }
}
