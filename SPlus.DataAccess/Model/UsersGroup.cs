namespace SPlus.DataAccess.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class UsersGroup
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int ID { get; set; }

        [Required]
        [StringLength(255)]
        public string UserName { get; set; }

        public int GroupID { get; set; }

        public virtual Group Group { get; set; }

        public virtual User User { get; set; }
    }
}
