namespace SPlus.Model.Domain
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
        public string ArabicName { get; set; }
        [Required]
        public string EnglishName { get; set; }
        
        public string EnglishDescription { get; set; }
        public string ArabicDescription { get; set; }
        [Required]
        public string EnglishEquation { get; set; }
        public string ArabicEquation { get; set; }
        [Required]
        public string DataSource { get; set; }
        [Required]
        public string Polarity { get; set; }
        [Required]
        public int CalculationMethod { get; set; }
        [Required]
        public string UnitOfMeasure { get; set; }
        public string EnglishUnitDetails { get; set; }
        public string ArabicUnitDetails { get; set; }
        [Required]
        public string Frequency { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public decimal Baseline { get; set; }
        [Required]
        public DateTime BaselineDate { get; set; }
        [Required]
        public string Champion { get; set; }
        [Required]
        public string Owner { get; set; }
        [Required]
        public int KPITypeID { get; set; }
        public string Code { get; set; }
        public int? StrategicObjectiveID { get; set; }
        public int? DivisionalObjectiveID { get; set; }

        public int? OrgStructureID { get; set; }
        public int? PerspectiveID { get; set; }
        public string Formula { get; set; }
        public decimal Weight { get; set; }
        public decimal BusinessUnitWeight { get; set; }
        public decimal DepartmentalWeight { get; set; }
        public string KPISource { get; set; }
        public int Years { get; set; }
        public string Direction { get; set; }
        public DateTime Modified { get; set; }
        public DateTime Created { get; set; }
        public bool IsLocked { get; set; }
        public bool IsConstantTargets { get; set; }

        [Column(TypeName = "date")]
        public DateTime? UnlockDate { get; set; }
        public bool RequireUpdate { get; set; }
        public bool ManualUnLock { get; set; }       

        #region Relation Models
        public virtual User ChampionModel { get; set; }
        public virtual User OwnerModel { get; set; }
        public virtual KPIType KPIType { get; set; }
        public virtual StrategicObjective StrategicObjective { get; set; }

        public virtual Perspective Perspective { get; set; }

        public virtual DivisionalObjective DivisionalObjective { get; set; }
        public virtual OrgStructure OrgStructure { get; set; }
     
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<KPIComment> KPIComments { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<KPIMeasure> KPIMeasures { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Parameter> Parameters { get; set; }
        #endregion

        #region Not Mapped
        [NotMapped]
        public bool CanUpdate { get; set; }
        [NotMapped]
        public bool IsCoded { get; set; }
        [NotMapped]
        public bool AllowLock { get; set; }
        [NotMapped]
        public bool IsDeletable { get; set; } = true;
        [NotMapped]
        public bool IsEditable { get; set; } = true;
        [NotMapped]
        public string Status { get; set; }
        [NotMapped]
        public decimal OutOfTarget { get; set; }
        [NotMapped]
        public decimal Target { get; set; }
        [NotMapped]
        public decimal Value { get; set; }
        [NotMapped]
        public List<Attachment> Attachments { get; set; } = new List<Attachment>();

        [NotMapped]
        public List<KPIsWeightDTO> KPIsWeight { get; set; }
        [NotMapped]
        public decimal AccumulutiveOutOfTarget { get; set; }
        [NotMapped]
        public string AccumulutiveStatus { get; set; }
        [NotMapped]
        public decimal? AccumulutiveValue { get; set; }
        [NotMapped]
        public decimal? AccumulutiveTarget { get; set; }
        #endregion

    }
    public class KPIsWeightDTO
    {
        public int ID { get; set; }
        public decimal Weight { get; set; }
    }
}
