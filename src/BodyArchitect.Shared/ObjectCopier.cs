using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using BodyArchitect.Shared;

namespace BodyArchitect.Client.Common
{
    /// <summary>
    /// Reference Article http://www.codeproject.com/KB/tips/SerializedObjectCloner.aspx
    /// 
    /// Provides a method for performing a deep copy of an object.
    /// Binary Serialization is used to perform the copy.
    /// </summary>

    public static class ObjectCopier
    {
        /// <summary>
        /// Perform a deep Copy of the object.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T Clone<T>(this T source, bool ignoreId = false)
        {
            //if (!typeof(T).IsSerializable)
            //{
            //    throw new ArgumentException("The type must be serializable.", "source");
            //}

            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            //IFormatter formatter = new BinaryFormatter();
            //formatter.Context = new StreamingContext(StreamingContextStates.Clone);
            //Stream stream = new MemoryStream();
            //using (stream)
            //{
            //    formatter.Serialize(stream, source);
            //    stream.Seek(0, SeekOrigin.Begin);
            //    return (T)formatter.Deserialize(stream);
            //}
            var tmp = SilverlightSerializer.IgnoreIds;
            SilverlightSerializer.IgnoreIds = ignoreId;
            var c = SilverlightSerializer.Serialize(source);
            var res = (T)SilverlightSerializer.Deserialize(c);
            SilverlightSerializer.IgnoreIds = tmp;
            return res;
        }

        public static T StandardClone<T>(this T source)
        {
            //if (!typeof(T).IsSerializable)
            //{
            //    throw new ArgumentException("The type must be serializable.", "source");
            //}

            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            IFormatter formatter = new BinaryFormatter();
            formatter.Context = new StreamingContext(StreamingContextStates.Clone);
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }

        }

        public static bool IsModified(this object currentCopy, object original)
        {
            string orgSum = SilverlightSerializer.GetChecksum(original);
            string copySum = SilverlightSerializer.GetChecksum(currentCopy);
            return orgSum != copySum;
        }
    }    
}
