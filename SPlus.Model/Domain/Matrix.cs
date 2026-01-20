namespace SPlus.Model.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Matrix")]
    public partial class Matrix
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int GroupID { get; set; }
        public virtual Group Group { get; set; }
        public int ResourceID { get; set; }
        public virtual Resource Resource { get; set; }
        public int RoleID { get; set; }
        public virtual Role Role { get; set; }
    }
}
