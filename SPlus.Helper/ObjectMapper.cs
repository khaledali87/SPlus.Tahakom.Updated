using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
namespace SPlus.Helper
{
    public static class ObjectMapper
    {
        public static decimal? TrimDecimal(this decimal? value)
        {
            if (value.HasValue)
                return value.Value / 1.000000m; // Removes unnecessary trailing zeros while preserving precision
            return value;
        }
        public static decimal TrimDecimal(this decimal value)
        {
            if (value == 0)
                return 0.0m;

            return value / 1.000000m; // Removes unnecessary trailing zeros while preserving precision
        }
        public static decimal? FormatDecimal(this decimal? value)
        {
            if (value.HasValue)
            {
                if (value >= 1)
                    return Math.Round(value.Value, 2, MidpointRounding.AwayFromZero).TrimDecimal(); // Round to 2 decimal places
                else if (value < 0.00001m)
                    return Math.Round(value.Value, 6, MidpointRounding.AwayFromZero).TrimDecimal(); // Round small numbers to 6 decimal places
                else
                    return value; // Keep as is if already formatted correctly
            }
            return value;

        }
        public static decimal FormatDecimal(this decimal value)
        {
            if (value >= 1)
                return (Math.Truncate(value * 100) / 100).TrimDecimal();
            //Math.Round(value, 2, MidpointRounding.AwayFromZero).TrimDecimal(); // Round to 2 decimal places
            else if (value < 0.01m)
                return Math.Round(value, 6, MidpointRounding.AwayFromZero).TrimDecimal(); // Round small numbers to 6 decimal places
            else
                return value; // Keep as is if already formatted correctly
        }
        public static object ContextMapper(this object oldobject, object newobject, List<PropertyInfo> properties)
        {
            object result = new object();
            foreach (var item in properties)
            {
                oldobject.GetType().GetProperty(item.Name).SetValue(oldobject, item.GetValue(newobject, default), default);
            }
            return oldobject;
        }
        public static IEnumerable<object> ContextListMapper(this IEnumerable<object> oldobjects, IEnumerable<object> newobjects, List<PropertyInfo> properties)
        {

            object result = new object();
            object oldobject;
            foreach (var newobject in newobjects.Where(w => (int)w.GetType().GetProperties().Where(x => x.Name?.ToLower() == "id").Select(s => s.GetValue(w)).FirstOrDefault() != 0))
            {
                oldobject = new object();
                int id = (int)newobject.GetType().GetProperty("ID").GetValue(newobject);
                oldobject = oldobjects.Where(w => (int)w.GetType().GetProperties().Where(x => x.Name?.ToLower() == "id").Select(s => s.GetValue(w)).FirstOrDefault() == id).FirstOrDefault();
                if (oldobject != null)
                    oldobject.ContextMapper(newobject, properties);

            }
            return oldobjects;
        }
    }
}
