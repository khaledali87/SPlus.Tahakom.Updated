namespace SPlus.Model.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Resource
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
       
        public string Name { get; set; }
        [Required]
       
        public string Code { get; set; }
        public bool IsScreen { get; set; }

        public string PropertyName { get; set; }

        public int? PropertyValue { get; set; }

        public int? LevelID { get; set; }

    }
}
