using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncJobs.Models
{
    public class UserInfo
    {
        private List<Group> _Groups = new List<Group>();
        public int UserID
        {
            set;
            get;
        }

        public string UserName
        {
            set;
            get;
        }
        public string Password
        {
            set;
            get;
        }

        public string SamAccountName
        {
            set;
            get;
        }

        public string DisplayName
        {
            set;
            get;
        }

        public string Email
        {
            set;
            get;
        }
        public string PhoneNumber
        {
            set;
            get;
        }
        public string Department
        {
            set;
            get;
        }
        public string UserProfilePicture
        {
            set;
            get;
        }
        public string SecurityToken
        {
            set;
            get;
        }
        public bool IsAdmin
        {
            set;
            get;
        }
        public bool IsSuperUser
        {
            set;
            get;
        }
        public string LoginPassword
        {
            get
            {
                if (SecurityToken != null)
                    return Encryption.GetCredentialsFromSecurityToken(SecurityToken)[1];
                else
                    return "";
            }
        }


        public bool IsDeletable
        {
            set;
            get;
        }
        public string GroupsFullName
        {
            get;
            set;
        }
        //public List<string> Groups
        //{
        //    set { _Groups = value; }
        //    get { return _Groups; }
        //} 
        public List<Group> Groups
        {
            set { _Groups = value; }
            get { return _Groups; }
        }
    }
}
