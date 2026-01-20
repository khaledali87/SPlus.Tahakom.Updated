using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace SPlus.Model.Domain
{
    [Table("KPIAffect")]
    public class KPIAffect
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public int KPIID { get; set; }
       
        public int Affected { get; set; }

       
        public int Effecting { get; set; }
    }
}
