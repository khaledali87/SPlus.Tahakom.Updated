using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class NotificationParameterDTO
    {
        public int ID { get; set; }

        public string Title { get; set; }

        public bool IsUser { get; set; }

        public string Value { get; set; }

        public int TemplateID { get; set; }
    }
}
