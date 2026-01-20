using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace SPlus.Model.Domain
{
    [Table("DivisionalObjective")]
    public partial class DivisionalObjective

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

        [NotMapped]
        public decimal Performance { get; set; }

        [NotMapped]
        public string Status { get; set; }

        [NotMapped]
        public decimal PreviousPerformance { get; set; }

        [NotMapped]
        public string PreviousStatus { get; set; }

        [NotMapped]
        public decimal VeriancePerformance { get; set; }

        [NotMapped]
        public string VerianceStatus { get; set; }


        public DateTime Modified { get; set; }

        public DateTime Created { get; set; }

        public decimal Weight { get; set; }

        public virtual StrategicObjective StrategicObjective { get; set; }

        public int StrategicObjectiveID { get; set; }

        public virtual OrgStructure OrgStructure { get; set; }

        public int? OrgStructureId { get; set; }
        
        public virtual ICollection<KPI> KPIs { get; set; }


    }
}
