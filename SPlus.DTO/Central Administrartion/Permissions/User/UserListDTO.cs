using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class UserListDTO
    {
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool IsDeletable { get; set; }
        public string Department { get; set; }

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
    }
}
