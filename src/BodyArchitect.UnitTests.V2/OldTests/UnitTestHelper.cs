using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using NHibernate;
using NUnit.Framework;
using BodyArchitect.Model;

namespace BodyArchitect.UnitTests.V2
{
    public static class UnitTestHelper
    {
        [STAThread]
        static void Main(string[] args)
        {
            //TestProfileFactory day = new TestProfileFactory();
            //day.createTestFixture();
            //            day.setupTest();

            //            day.Test1();
        }
        #region Run Method

        /// <summary>
        ///	Runs a method on a type, given its parameters. This is useful for
        ///	calling private methods.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="strMethod"></param>
        /// <param name="aobjParams"></param>
        /// <returns>The return value of the called method.</returns>
        public static object RunStaticMethod(System.Type t, string strMethod, object[] aobjParams)
        {
            BindingFlags eFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            return RunMethod(t, strMethod, null, aobjParams, eFlags);
        } //end of method

        public static object RunInstanceMethod(System.Type t, string strMethod, object objInstance, object[] aobjParams)
        {
            BindingFlags eFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            return RunMethod(t, strMethod, objInstance, aobjParams, eFlags);
        } //end of method

        private static object RunMethod(System.Type t, string strMethod, object objInstance, object[] aobjParams, BindingFlags eFlags)
        {
            MethodInfo m;

            m = t.GetMethod(strMethod, eFlags);
            if (m == null)
            {
                throw new ArgumentException("There is no method '" + strMethod + "' for type '" + t.ToString() + "'.");
            }

            object objRet = m.Invoke(objInstance, aobjParams);
            return objRet;

        } //end of method

        #endregion

        static public int GetQueryCount(this ISession session)
        {
            return (int)(session.SessionFactory.Statistics.QueryExecutionCount);
        }

        public static bool CompareDateTime(this DateTime date1,DateTime date2)
        {
            return date1.Date == date2.Date && date1.Hour == date2.Hour && date1.Minute==date2.Minute && date1.Second ==date2.Second;
        }

        static bool shouldBeSkipped(string propertyName)
        {
            return propertyName == "Version" || propertyName == "Id" || propertyName == "GlobalId" || propertyName == "IsNew";
        }
        public static void CompareObjects(object original, object secondObject,bool skipIdAndVersion=false)
        {
            Type type = original.GetType();
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo info in properties)
            {
                if (canCompareProperty(info) && (!skipIdAndVersion || !shouldBeSkipped(info.Name)))
                {
                    object value1 = info.GetValue(original, null);
                    object value2 = info.GetValue(secondObject, null);
                    if(value2 is DateTime)
                    {
                        continue;
                    }
                    else
                    {
                        Assert.AreEqual(value1, value2, "Property: " + info.Name + " has different values");    
                    }
                    
                }
                if(info.PropertyType.GetInterface("ICollection")!=null)
                {
                    var col1 = (IList)info.GetValue(original, null);
                    var col2 = (IList)info.GetValue(secondObject, null);
                    Assert.AreEqual(col1.Count,col2.Count);
                    for (int i = 0; i < col1.Count; i++)
                    {
                        CompareObjects(col1[i], col2[i], skipIdAndVersion);
                    }
                }
            }
        }

        private static bool canCompareProperty(PropertyInfo info)
        {
            return info.CanRead && ((!info.PropertyType.IsClass && !info.PropertyType.IsInterface) || info.PropertyType == typeof(string));
        }

        public static void SetField(object obj, string fieldName, object value)
        {
            MemberInfo[] info = obj.GetType().GetMember(fieldName, BindingFlags.Instance | BindingFlags.SetField | BindingFlags.Public | BindingFlags.NonPublic);
            if (info.Length > 0)
            {
                FieldInfo fieldInfo = (FieldInfo)info[0];
                fieldInfo.SetValue(obj, value);
            }
            else
            {
                throw new ArgumentException("Wrong fieldName", fieldName);
            }
        }

        public static void SetProperty(object obj, string fieldName, object value)
        {
            MemberInfo[] info = obj.GetType().GetMember(fieldName, BindingFlags.Instance | BindingFlags.SetField | BindingFlags.Public | BindingFlags.NonPublic);
            if (info.Length > 0)
            {
                var fieldInfo = (PropertyInfo)info[0];
                fieldInfo.SetValue(obj, value,null);
            }
            else
            {
                throw new ArgumentException("Wrong fieldName", fieldName);
            }
        }

        public static T CreateCopy<T>(T obj)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, obj);
            stream.Seek(0, SeekOrigin.Begin);
            T container = (T)formatter.Deserialize(stream);
            return container;
        }


        static object getRandomEnumValue(Type enumType)
        {
            int[] values =(int[])Enum.GetValues(enumType);
            Random rand = new Random();
            return values[rand.Next(0, values.Length - 1)];
        }
        public static void TestClone(ICloneable cloneableObject)
        {
            Type type = cloneableObject.GetType();
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            Random rand = new Random();
            foreach (PropertyInfo info in properties)
            {
                
                if (info.CanWrite)
                {
                    if (info.PropertyType == typeof (string))
                    {
                        info.SetValue(cloneableObject, Guid.NewGuid().ToString(), null);
                    }
                    else if (info.PropertyType == typeof (Guid))
                    {
                        info.SetValue(cloneableObject, Guid.NewGuid(), null);
                    }
                    else if (info.PropertyType == typeof(DateTime))
                    {
                        info.SetValue(cloneableObject, DateTime.Now.AddSeconds(rand.Next(1000)), null);
                    }
                    else if (info.PropertyType.IsEnum)
                    {
                        info.SetValue(cloneableObject, getRandomEnumValue(info.PropertyType), null);
                    }
                    else if (info.PropertyType == typeof(bool))
                    {
                        //set oposit value to initial value
                        bool defaultValue = (bool)info.GetValue(cloneableObject, null);
                        info.SetValue(cloneableObject, !defaultValue, null);
                    }
                    else if (info.PropertyType == typeof(int) || info.PropertyType == typeof(decimal) || info.PropertyType == typeof(long) || info.PropertyType == typeof(short) || info.PropertyType == typeof(float) ||
                             info.PropertyType == typeof (double))
                    {
                        
                        info.SetValue(cloneableObject, Convert.ChangeType(rand.Next(1, 10000), info.PropertyType),
                                      null);
                    }
                }
            }
            object clonedObject=cloneableObject.Clone();

            CompareObjects(cloneableObject, clonedObject);
        }

    }
}
