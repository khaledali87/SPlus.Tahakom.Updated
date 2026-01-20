namespace SPlus.Model.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SYNC_Job_Logging
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

       
        public string Title { get; set; }

       
        public string SyncStartAt { get; set; }

       
        public string SyncEndAt { get; set; }

        public DateTime? Modified { get; set; }

        public DateTime? Created { get; set; }
    }
}
