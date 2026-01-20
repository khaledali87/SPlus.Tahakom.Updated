namespace SPlus.Model.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Workflow")]
    public partial class Workflow
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Key]
        public int WorkflowID { get; set; }
        public int BaseWorkflowID { get; set; }
        [Required]
        public string Name { get; set; }

        public int? KPITypeID { get; set; }

        public bool IsDeleted { get; set; }

        [Column(TypeName = "date")]
        public DateTime Created { get; set; }

        [Column(TypeName = "date")]
        public DateTime Modified { get; set; }

        public virtual ICollection<WorkflowStep> WorkflowSteps { get; set; }
        public virtual KPIType KPIType { get; set; }
        public virtual BaseWorkflow BaseWorkflow { get; set; }


    }
}
