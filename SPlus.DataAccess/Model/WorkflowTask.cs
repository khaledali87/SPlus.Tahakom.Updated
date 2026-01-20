namespace SPlus.DataAccess.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class WorkflowTask
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int ID { get; set; }

        [StringLength(255)]
        public string Title { get; set; }

        [StringLength(255)]
        public string InstanceID0 { get; set; }

        [StringLength(255)]
        public string Status { get; set; }

        [StringLength(255)]
        public string AssignedTo { get; set; }

        [StringLength(255)]
        public string StepID { get; set; }

        [StringLength(255)]
        public string EscalateLevel { get; set; }

        public string Comments { get; set; }

        public DateTime? LastRemiderDate { get; set; }

        public DateTime? LastEscalationDate { get; set; }

        [StringLength(255)]
        public string TaskCreatedDate { get; set; }

        public DateTime? Modified { get; set; }

        public DateTime? Created { get; set; }
    }
}
