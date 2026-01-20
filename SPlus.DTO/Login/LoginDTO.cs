using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class LoginDTO
    {
        public UserDTO UserData { set; get; }
        public string Token { set; get; }
        public List<string> Screens { get; set; } = new List<string>();
        public string SSOToken { set; get; }
        public bool IsTwoFactorEnabled { get; set; }
    }
}
