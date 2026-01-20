namespace SPlus.Model.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("WFReminderRegistry")]
    public partial class WFReminderRegistry
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int ID { get; set; }

        public int RelatedItemID { get; set; }

        [Required]
        public string Type { get; set; }

        [Column(TypeName = "date")]
        public DateTime ReminderDate { get; set; }
    }
}
