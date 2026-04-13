using SPlus.Helper;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class JsonExtensions
    {
        [DbFunction("dbo", "JsonValue")]
        public static string JsonValue(string json, string path)
        {
            throw new NotSupportedException();
        }
    }
}
