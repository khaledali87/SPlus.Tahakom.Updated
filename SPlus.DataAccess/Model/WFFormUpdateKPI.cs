namespace SPlus.DataAccess.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;


    [Table("WFFormUpdateKPI")]
    public class WFFormUpdateKPI
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int ID { get; set; }
        [Required]
        public int MeasureID { get; set; }

        [Required]
        public virtual WFRequest WFRequest { get; set; }
        [Required]
        public decimal Value { get; set; }
        [Required]
        public decimal OldValue { get; set; }
        [Required]
        public DateTime DueDate { get; set; }
        [Required]
        public decimal Target { get; set; }

        [NotMapped]
        public Attachment Attachment { get; set; } = new Attachment();
    }
}
