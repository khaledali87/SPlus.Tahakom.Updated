namespace SPlus.DataAccess.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class NotificationReceiver
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public int TemplateID { get; set; }

        [Required]
        [StringLength(255)]
        public string Receiver { get; set; }

        [Required]
        public bool IsGroup { get; set; }

        [Required]
        public bool IsCC { get; set; }

        [NotMapped]
        public List<User> Users { get; set; } = new List<User>();

        public virtual NotificationConfiguration NotificationConfiguration { get; set; }

    }
}
