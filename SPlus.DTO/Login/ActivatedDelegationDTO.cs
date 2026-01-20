using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class ActivatedDelegationDTO
    {
        public string SSOToken { set; get; }
        public UserDTO UserData { set; get; }
        public string Token { set; get; }
        public List<string> Screens { get; set; } = new List<string>();
    }
}
