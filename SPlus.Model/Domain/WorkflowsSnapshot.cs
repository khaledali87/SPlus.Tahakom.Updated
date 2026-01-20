namespace SPlus.Model.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("WorkflowsSnapshot")]
    public partial class WorkflowsSnapshot
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int ID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Type { get; set; }

        [Column(TypeName = "date")]
        public DateTime Created { get; set; }

        [Column(TypeName = "date")]
        public DateTime Modified { get; set; }
    }
}
