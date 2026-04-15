using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.BLL.Caching
{
    public class CacheService
    {
        private static readonly ObjectCache cache = MemoryCache.Default;

        public static T GetOrSet<T>(string key, Func<T> getData, int minutes = 10)
        {
            if (cache.Contains(key))
            {
                return (T)cache[key];
            }

            var data = getData();

            cache.Set(key, data, DateTimeOffset.Now.AddMinutes(minutes));

            return data;
        }

        public static void Reset(string key)
        {
            MemoryCache.Default.Remove("users");
        }
        
    }
}
