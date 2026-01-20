namespace SPlus.Model.Domain
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

       
        public string Title { get; set; }

       
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }

       
        public string SenderEmail { get; set; }

       
        public string SenderPassword { get; set; }

        public bool EnableSsl { get; set; }

        public bool UsingSharepointSMTP { get; set; }

       
        public string SiteURL { get; set; }

        public DateTime? Modified { get; set; }

        public DateTime? Created { get; set; }
    }
}
