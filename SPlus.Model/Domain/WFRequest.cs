namespace SPlus.Model.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class WFRequest
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int ID { get; set; }
        [Required]
        public int WorkflowID { get; set; }
        [Required]
        public int BaseWorkflowID { get; set; }
        [Required]
        public string RequestedBy { get; set; }

        [Required]
        public int Status { get; set; }

        [Column(TypeName = "date")]
        public DateTime Created { get; set; }

        [Column(TypeName = "date")]
        public DateTime Modified { get; set; }
        public virtual ICollection<WFRequestStep> WFRequestSteps { get; set; }

        public virtual WFFormUpdateKPI WFFormUpdateKPI { get; set; }
        [NotMapped]
        public Attachment Attachment { get; set; } = new Attachment();

        [NotMapped]
        public bool CanApprove { get; set; }
    }
}
