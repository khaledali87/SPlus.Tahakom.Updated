using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class UserDTO
    {
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string Department { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }

        private string userProfilePicture;
        public string UserProfilePicture
        {
            get
            {
                if (userProfilePicture == null)
                {
                    userProfilePicture = "UserPhoto/" + UserName;
                }
                return userProfilePicture;
            }
            set
            {
                userProfilePicture = value;
            }
        }
        public bool isAdmin { get; set; }
        public bool IsDeletable { get; set; }
        public virtual List<GroupDTO> Groups { get; set; }
        public int SMSAuthCode { get; set; }
    }
}
