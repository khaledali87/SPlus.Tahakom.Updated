namespace SPlus.DataAccess.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EmailConfiguration")]
    public partial class EmailConfiguration
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [StringLength(255)]
        public string Title { get; set; }

        [StringLength(255)]
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }

        [StringLength(255)]
        public string SenderEmail { get; set; }

        [StringLength(255)]
        public string SenderPassword { get; set; }

        public bool EnableSsl { get; set; }

        public bool UsingSharepointSMTP { get; set; }

        [StringLength(255)]
        public string SiteURL { get; set; }

        public DateTime? Modified { get; set; }

        public DateTime? Created { get; set; }
    }
}
