using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class BaseDTO 
    {
        public int ID { get; set; }
        public string EnglishName { get; set; }
        public string ArabicName { get; set; }
        public bool IsDeletable { get; set; }
    }
    public class BaseLevelDTO :  BaseDTO
    {
        public bool CanAccess { get; set; }
    }
}
