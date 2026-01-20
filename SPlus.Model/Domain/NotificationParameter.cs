namespace SPlus.Model.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class NotificationParameter
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public string Title { get; set; }

        public bool IsUser { get; set; }

        [Required]
        public string Value { get; set; }

        public int TemplateID { get; set; }
        public virtual NotificationConfiguration NotificationConfiguration { get; set; }
    }
}
