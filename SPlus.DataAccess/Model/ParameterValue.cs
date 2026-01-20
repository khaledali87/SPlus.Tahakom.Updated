namespace SPlus.DataAccess.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ParameterValue
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public decimal Value { get; set; }

        [Required]
        public int MeasureID { get; set; }
        [Required]
        public int ParameterID { get; set; }
        [Required]
        
        public DateTime Modified { get; set; }
        [Required]

        public DateTime Created { get; set; }
        public virtual KPIMeasure Measure { get; set; }


    }
}
