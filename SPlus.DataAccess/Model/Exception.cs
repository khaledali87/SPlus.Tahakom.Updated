namespace SPlus.DataAccess.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Exception
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [StringLength(255)]
        public string Title { get; set; }

        public string Message { get; set; }

        [StringLength(255)]
        public string Controller { get; set; }

        [StringLength(255)]
        public string Function { get; set; }

        public DateTime? Modified { get; set; }

        public DateTime? Created { get; set; }
    }
}
