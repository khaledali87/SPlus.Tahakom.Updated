namespace SPlus.Model.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Request")]
    public partial class Request
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public int WorkflowID { get; set; }

        [Required]
        public string CreatedBy { get; set; }

        [Required]
        public DateTime Created { get; set; }

        [Required]
        public DateTime Modified { get; set; }

        [Required]
        public int Status { get; set; }

        [Required]
        public string Form { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public int RelatedRequestID { get; set; }

        public virtual ICollection<RequestStep> RequestSteps { get; set; }



        [NotMapped]
        public bool CanApprove { get; set; }
        [NotMapped]
        public Attachment Attachment { get; set; }

        [NotMapped]
        public List<Attachment> Attachments { get; set; }
        [NotMapped]
        public UpdateKPIForm UpdateKPIForm { get; set; }

        [NotMapped]
        public int RelatedItemId { get; set; }
    }
}