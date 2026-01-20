namespace SPlus.DataAccess.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("KPIThreshold")]
    public partial class KPIThreshold
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        
        public int KPITypeID { get; set; }

        public string Code { get; set; } 

        public int? Min { get; set; } 

        public int? Max { get; set; }

        [StringLength(10)] 
        public string MinOperator { get; set; }

        [StringLength(10)]
        public string Operator { get; set; }
        
        public virtual KPIType KPIType { get; set; }
        public virtual Status Status { get; set; }

    }
}
