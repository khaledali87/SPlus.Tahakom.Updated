namespace SPlus.Model.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Linq;

    [Table("Strategy")]
    public partial class Strategy
    {
        public Strategy()
        {
            Themes = new HashSet<Theme>();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public string EnglishName { get; set; }

        [Required]
        public string ArabicName { get; set; }

        [Required]
        public bool IsSloganEnabled { get; set; }

        [Required]
        public string EnglishSlogan { get; set; }

        [Required]
        public string ArabicSlogan { get; set; }

        [Required]
        public bool IsVisionEnabled { get; set; }

        [Required]
        public string EnglishVision { get; set; }

        [Required]
        public string ArabicVision { get; set; }

        [Required]
        public bool IsMissionEnabled { get; set; }

        [Required]
        public string EnglishMission { get; set; }

        [Required]
        public string ArabicMission { get; set; }    

        [Required]        
        public DateTime Created { get; set; }

        [Required]        
        public DateTime Modified { get; set; }
        public string YearString { get; set; }

        [NotMapped]
        public List<string> Years
        {
            get
            {
                if (string.IsNullOrEmpty(YearString))
                    return new List<string>();
                return YearString.Split(',').ToList();
            }
        }

        [NotMapped]
        public bool IsDeletable { get; set; } = false;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Theme> Themes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Perspective> Perspectives { get; set; }
        [NotMapped]
        public Attachment Attachment { get; set; }
    }
}