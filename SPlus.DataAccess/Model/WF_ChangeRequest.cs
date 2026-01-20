namespace SPlus.DataAccess.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class WF_ChangeRequest
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int ID { get; set; }

        public int? RequestID { get; set; }

        public int? KPIID { get; set; }

        public int? MeasureID { get; set; }

        public decimal? NewTarget { get; set; }

        public decimal? OldTarget { get; set; }
    }
}
