namespace SPlus.Model.Domain
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

       
        public string Title { get; set; }

       
        public string InstanceID0 { get; set; }

       
        public string Status { get; set; }

       
        public string AssignedTo { get; set; }

       
        public string StepID { get; set; }

       
        public string EscalateLevel { get; set; }

        public string Comments { get; set; }

        public DateTime? LastRemiderDate { get; set; }

        public DateTime? LastEscalationDate { get; set; }

       
        public string TaskCreatedDate { get; set; }

        public DateTime? Modified { get; set; }

        public DateTime? Created { get; set; }
    }
}
