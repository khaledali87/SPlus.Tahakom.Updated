namespace SPlus.Model.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RequestStep")]
    public partial class RequestStep
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int ID { get; set; }

        [Required]
        public  int RequestID { get; set; }

        public virtual Request Request { get; set; }

        [Required]
        public string EnglishName { get; set; }

        [Required]
        public string ArabicName { get; set; }
        public bool IsGroup { get; set; }

        [Required]
        public int Order { get; set; }

        [Required]
        public string Approver { get; set; }

        [Required]
        public int Status { get; set; }

        public string ActionBy { get; set; }

        public virtual User ActionByModel { get; set; }

        public string Comments { get; set; }
        
        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }
        public int QualityType { get; set; }

        public bool IsCancelled { get; set; }

        [NotMapped]
        public bool CanApprove { get; set; }               
    }
}
