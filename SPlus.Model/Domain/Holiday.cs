namespace SPlus.Model.Domain
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Holiday
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string ArabicName { get; set; }
        public string EnglishName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public bool? IsActive { get; set; } = true;


        public DateTime Modified { get; set; }
        public DateTime Created { get; set; }
    }

}
