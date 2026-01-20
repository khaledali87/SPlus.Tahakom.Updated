using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace SPlus.Model.Domain
{
    public class BaseLevel
    {
        [NotMapped]
        public bool CanAccess { get; set; }
    }
}
