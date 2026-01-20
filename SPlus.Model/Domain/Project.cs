namespace SPlus.Model.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    
    public class Project
    {    
        public int ID { get; set; }
        public Guid ProjectUID { get; set; }
        public string EnglishName { get; set; }
        public string ArabicName { get; set; }
        public string Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public User Manager { get; set; }
        public decimal Progress { get; set; }
        public int DepartmentID { get; set; }
        public bool IsOperational { get; set; }
    }
}
