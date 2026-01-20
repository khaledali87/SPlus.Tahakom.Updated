using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SyncJobs
{
    public static partial class Extentions
    {
        public static string ToDateFormat(this DateTime val)
        {
            return val.ToString("dd/MM/yyyy");
        }

        public static string ToDateFormat(this DateTime? val)
        {
            return val.HasValue ? val.Value.ToString("dd/MM/yyyy") : "-";
        }

        public static string ToEcryptedString(this string val)
        {
            return Encryption.Encrypt(val);
        }

        public static string ToDecryptedString(this string val)
        {
            return Encryption.Decrypt(val);
        }

        public static bool HasColumn(this IDataRecord dr, string columnName)
        {
            for (int i = 0; i < dr.FieldCount; i++)
            {
                if (dr.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }
            return false;
        }

        public static DateTime ServiceMinValue(this DateTime val)
        {
            return DateTime.MinValue.AddDays(1);
        }

       
    }
}
