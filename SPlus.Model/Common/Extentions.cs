using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Web;
using System.Text.RegularExpressions;
using System.Linq.Expressions;
using System.Reflection;

namespace SPlus.Model.Common
{
    public static partial class Extentions
    {

        public static string ToEcryptedString(this string val)
        {
            return Encryption.Encrypt(val);
        }

        public static string ToDecryptedString(this string val)
        {
            return Encryption.Decrypt(val);
        }

    }


}
