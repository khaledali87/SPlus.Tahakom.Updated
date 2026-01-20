namespace SPlus.Model.Domain
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
        [Required]
        public string EnglishName { get; set; }
        [Required]
        public string ArabicName { get; set; }
        [Required]
        public string AssignedTo { get; set; }
        [Required]
        public int Status { get; set; }
        [Required]
        public int RelatedItemID { get; set; }
        [Required]
        public string NotificationType { get; set; }
        [Required]
        public string ItemType { get; set; }
        [Required]
        public DateTime Modified { get; set; }
        [Required]
        public DateTime Created { get; set; }
    }
}
