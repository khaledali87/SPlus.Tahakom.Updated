namespace SPlus.Model.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Linq;

    [Table("OrgStructure")]
    public partial class OrgStructure
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public string EnglishName { get; set; }
        [NotMapped]
        public bool IsDeletable { get; set; }
        [Required]
        public string ArabicName { get; set; }
        public int? ParentID { get; set; }
        public decimal Weight { get; set; }
        public string Manager { get; set; }
        public User ManagerModel { get; set; }
        public int? GroupID { get; set; }
        public Group Group { get; set; }
        public string Abbreviation { get; set; }
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
        public virtual List<OrgStructure> Childrens { get; set; }
        [NotMapped]
        public virtual OrgStructure Parent { get; set; }
        [NotMapped]
        public decimal Performance { get; set; }
        [NotMapped]
        public string Status { get; set; }

        [NotMapped]
        public decimal PreviousPerformance { get; set; }
        [NotMapped]
        public string PreviousStatus { get; set; }


        [NotMapped]
        public decimal Variance { get; set; }
        [NotMapped]
        public decimal VariancePerformance { get; set; }
        [NotMapped]
        public string VarianceStatus { get; set; }

        [NotMapped]
        public decimal AvragePerformance { get; set; }
        [NotMapped]
        public string AvrageStatus { get; set; }


        [NotMapped]
        public int KPIsCount { get; set; }


        [NotMapped]
        public List<KPI> KPIs { get; set; }

        [NotMapped]
        public int DepartmentsCount { get; set; }

        [NotMapped]
        public int InitiativesCount { get; set; }
        [NotMapped]
        public int ProjectsCount { get; set; }
        [NotMapped]
        public int FilesCount { get; set; }
        [NotMapped]
        public bool IsCorporate { get; set; }

        public virtual ICollection<DivisionalObjective> DivisionalObjective { get; set; }

        public virtual ICollection<KPI> KPIList { get; set; }
    }
}
