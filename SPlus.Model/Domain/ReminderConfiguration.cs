namespace SPlus.Model.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ReminderConfiguration")]
    public partial class ReminderConfiguration
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int KPITypeID { get; set; }

        public int BeforeReminder { get; set; }
        
        public int FirstReminder { get; set; }

        public int SecondReminder { get; set; }

        public virtual KPIType KPIType { get; set; }
    }
}
