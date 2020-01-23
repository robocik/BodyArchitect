using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Client.Common
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Property)]
    public class NotCloneableAttribute : Attribute
    {
    }

    [Serializable]
    [AttributeUsage(AttributeTargets.Property)]
    public class SkipCloneableAttribute : Attribute
    {

    }

    public class CloneCleaner
    {
        public static void Clean(object obj)
        {
            Clean(obj, new Dictionary<object, bool>());
        }

        public static void Clean(object obj, Dictionary<object, bool> visited)
        {
            if (obj == null)
                return;

            if (visited.ContainsKey(obj))
                return;

            visited.Add(obj, true);

            var type = obj.GetType();

            var properties = type.GetProperties();

            foreach (var propertyInfo in properties)
            {
                var propertyType = propertyInfo.PropertyType;

                var skipAttribute = propertyInfo.GetCustomAttributes(typeof(SkipCloneableAttribute), true).FirstOrDefault();

                if (skipAttribute != null)
                    continue;

                var attribute = propertyInfo.GetCustomAttributes(typeof(NotCloneableAttribute), true).FirstOrDefault();

                if (attribute != null)
                {
                    if (propertyInfo.CanWrite)
                    {
                        var value = GetDefaultValue(propertyType);
                        propertyInfo.SetValue(obj, value, null);
                    }
                }
                else if (!propertyType.IsValueType && propertyType != typeof(string) && propertyInfo.CanRead)
                {
                    try
                    {
                        var value = propertyInfo.GetValue(obj, null);

                        if (value is IEnumerable)
                        {
                            var collection = (IEnumerable)value;

                            foreach (var item in collection)
                            {
                                Clean(item, visited);
                            }
                        }
                        else
                        {
                            Clean(value, visited);
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

        private static object GetDefaultValue(Type propertyType)
        {
            if (propertyType.IsValueType)
            {
                return Activator.CreateInstance(propertyType);
            }

            return null;
        }
    }
}
