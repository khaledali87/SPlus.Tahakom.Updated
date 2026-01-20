namespace SPlus.Model.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EmailTemplate")]
    public partial class EmailTemplate
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

       
        public string Title { get; set; }

        public string Body { get; set; }

       
        public string Subject { get; set; }

        public DateTime? Modified { get; set; }

        public DateTime? Created { get; set; }

        public string Type { get; set; }

        public string To { get; set; }
        [NotMapped] 
        public List<string> CC { get; set; }
    }
}
