namespace SPlus.DataAccess.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("KPI")]
    public partial class KPI : BaseLevel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public KPI()
        {
            KPIComments = new HashSet<KPIComment>();
            KPIMeasures = new HashSet<KPIMeasure>();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [StringLength(255)]
        public string EnglishName { get; set; }

        [Required]
        [StringLength(255)]
        public string ArabicName { get; set; }

        [Required]
        [StringLength(255)]
        public string ReferenceNo { get; set; }

        [Required]
        public string EnglishDescription { get; set; }

        [Required]
        public string ArabicDescription { get; set; }

        [StringLength(255)]
        public string Formula { get; set; }

        [Required]
        [StringLength(255)]
        public string Champion { get; set; }

        [Required]
        [StringLength(255)]
        public string Owner { get; set; }

        [Required]
        [StringLength(255)]
        public string Sponser { get; set; }

        [Required]
        [StringLength(255)]
        public string DataSource { get; set; }

        public int KPITypeID { get; set; }

        [Required]
        [StringLength(255)]
        public string UnitOfMeasure { get; set; }

        [Required]
        [StringLength(255)]
        public string Polarity { get; set; }

        [Required]
        [StringLength(255)]
        public string Frequency { get; set; }

        [Required]
        [StringLength(255)]
        public string StartDate { get; set; }

        public decimal Baseline { get; set; }
        public decimal Weight { get; set; }
        public DateTime BaselineDate { get; set; }

        [Required]
        [StringLength(255)]
        public string Years { get; set; }

        [StringLength(255)]
        public string Direction { get; set; }

        [StringLength(255)]
        public string EnglishUnitDetails { get; set; }

        [StringLength(255)]
        public string ArabicUnitDetails { get; set; }

        public string EnglishEquation { get; set; }

        public string ArabicEquation { get; set; }

        public DateTime Modified { get; set; }

        public DateTime Created { get; set; }

        public int StrategicObjectiveID { get; set; }

        public bool IsLocked { get; set; }

        [Column(TypeName = "date")]
        public DateTime? UnlockDate { get; set; }

        public int OrgStructureID { get; set; }
        public bool RequireUpdate { get; set; }

        public virtual User ChampionModel { get; set; }

        public virtual OrgStructure OrgStructure { get; set; }

        public virtual User OwnerModel { get; set; }

        public virtual User SponserModel { get; set; }
        [NotMapped]
        public List<Attachment> Attachments { get; set; } = new List<Attachment>();
        public virtual StrategicObjective StrategicObjective { get; set; }
        public virtual KPIType KPIType { get; set; } 

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<KPIComment> KPIComments { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<KPIMeasure> KPIMeasures { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Parameter> Parameters { get; set; }
    }
}
