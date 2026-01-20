using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;


namespace SPlus.DataAccess.Model
{
    [Table("AuditTrail")]
    public class AuditTrail
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int ItemID { get; set; }
        public string ItemType { get; set; }
        public string Action { get; set; }
        public string UserName { get; set; }

    }
}
