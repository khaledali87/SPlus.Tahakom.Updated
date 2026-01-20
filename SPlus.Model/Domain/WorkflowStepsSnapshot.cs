namespace SPlus.Model.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("WorkflowStepsSnapshot")]
    public partial class WorkflowStepsSnapshot
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int ID { get; set; }

        [Required]
        public string EnglishName { get; set; }

        [Required]
        public string ArabicName { get; set; }

        [Required]
        public string Approver { get; set; }

        public int WorkflowID { get; set; }

        public bool IsGroup { get; set; }

        public int Order { get; set; }

        [Column(TypeName = "date")]
        public DateTime Created { get; set; }

        [Column(TypeName = "date")]
        public DateTime Modified { get; set; }
    }
}
