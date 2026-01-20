using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model.Domain
{
    [Table("Session")]
    public partial class Session
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string UserName { get; set; }
        public Guid SessionID { get; set; }
        public DateTime SessionTime { get; set; }


    }
}
