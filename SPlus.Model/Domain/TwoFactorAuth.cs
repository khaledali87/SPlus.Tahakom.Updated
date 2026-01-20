using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model.Domain
{
   public class TwoFactorAuth
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public DateTime Created { get; set; }

        [Required]
        public DateTime Expired { get; set; }

        [Required]
        public int SMSAuthCode { get; set; }

     
        public string MobileNumber { get; set; }
        [Required]
        public int Counter { get; set; }

        [Required]
        public string UserName { get; set; }
      
        public string Token { get; set; }

    }
}
