namespace SPlus.DataAccess.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Notification
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [StringLength(255)]
        public string EnglishName { get; set; }

        [StringLength(255)]
        public string ArabicName { get; set; } 

        [StringLength(255)]
        public string AssignedTo { get; set; }

        [StringLength(255)]
        public string Status { get; set; }

        public decimal? KPIID { get; set; }

        public decimal? DelegationID { get; set; }

        [StringLength(255)]
        public string KPIName { get; set; }

        [StringLength(255)]
        public string NotificationType { get; set; }

        public decimal? TaskID { get; set; }

        public DateTime? Modified { get; set; }

        public DateTime? Created { get; set; }
    }
}
