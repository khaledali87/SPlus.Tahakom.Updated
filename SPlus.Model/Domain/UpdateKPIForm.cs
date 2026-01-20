namespace SPlus.Model.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public class UpdateKPIForm
    {
        public int BaseWorkflow { get; set; }
        public int Type { get; set; }
        public int RelatedID { get; set; }
        public decimal Value { get; set; }
        public decimal OldValue { get; set; }
        public decimal Target { get; set; }
        public string UnitOfMeasure { get; set; }
        public string EnglishUnitDetails { get; set; }
        public string ArabicUnitDetails { get; set; }
        public DateTime DueDate { get; set; }
        
        public Attachment Attachment { get; set; }
        public List<Attachment> Attachments { get; set; }
        [NotMapped]
        public List<Parameter> Parameters { get; set; }
    }
}
