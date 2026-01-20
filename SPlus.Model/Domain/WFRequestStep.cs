namespace SPlus.Model.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("WFRequestStep")]
    public partial class WFRequestStep
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int ID { get; set; }
        
        public  int RequestID { get; set; }

        public string EnglishName { get; set; }
        public string ArabicName { get; set; }
        [Required]
        public int Order { get; set; }

        [Required]
        public string Approver { get; set; }
        public int WorkflowStepID { get; set; }
        public string ActionBy { get; set; }
        public string Comments { get; set; }
        public bool IsGroup { get; set; }
        [Required]
        public int Status { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        [NotMapped]
        public bool CanApprove { get; set; }
        public virtual WFRequest WFRequest { get; set; }
        public virtual User ActionByModel { get; set; }
        [NotMapped]
        public virtual List<WorkflowStep> WorkflowSteps { get; set; } = new List<WorkflowStep>();
    }
}
