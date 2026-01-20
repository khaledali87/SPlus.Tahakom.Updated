using SPlus.Helper;
using SPlus.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;

namespace SPlus.API.Attribute
{
    public class DataFormatingInvoker : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Response?.Content != null)
            {
                var content = actionExecutedContext.Response.Content as ObjectContent;
                if (content?.Value != null)
                {
                    // Check if response is a ResultWrapper<T> and process its Data
                    var resultType = content.Value.GetType();
                    if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(ResultWrapper<>))
                    {
                        var dataProperty = resultType.GetProperty("Data");
                        var dataValue = dataProperty?.GetValue(content.Value);

                        if (dataValue != null)
                        {
                            dataProperty.SetValue(content.Value, FormatDecimals(dataValue));
                        }
                    }
                }
            }
        }
        private object FormatDecimals(object value, int currentDepth = 0, int maxDepth = 10)
        {
            if (value == null || currentDepth > maxDepth) // Stop if null or max depth reached
                return value;

            if (value is decimal decimalValue)
                return ObjectMapper.FormatDecimal(decimalValue);

            if (value is IEnumerable enumerable) // Handle lists, including nested lists
            {
                Type itemType = value.GetType().GetGenericArguments().FirstOrDefault();
                if (itemType != null)
                {
                    var formattedList = Activator.CreateInstance(typeof(List<>).MakeGenericType(itemType)) as IList;
                    foreach (var item in enumerable)
                    {
                        formattedList.Add(FormatDecimals(item, currentDepth + 1, maxDepth));
                    }
                    return formattedList;
                }
            }

            var type = value.GetType();
            if (!type.IsPrimitive && !type.IsEnum && type != typeof(string))
            {
                var newObj = Activator.CreateInstance(type); // Clone object

                foreach (var prop in type.GetProperties())
                {
                    if (!prop.CanWrite) // Skip read-only properties
                        continue;

                    var propValue = prop.GetValue(value);
                    if (propValue == null)
                        continue;

                    if (prop.PropertyType == typeof(decimal) || prop.PropertyType == typeof(decimal?))
                    {
                        prop.SetValue(newObj, ObjectMapper.FormatDecimal((decimal)propValue));
                    }
                    else if (typeof(IEnumerable).IsAssignableFrom(prop.PropertyType))
                    {
                        prop.SetValue(newObj, FormatDecimals(propValue, currentDepth + 1, maxDepth)); // Recursively format lists
                    }
                    else
                    {
                        prop.SetValue(newObj, propValue);
                    }
                }

                return newObj;
            }

            return value;
        }



    }
}