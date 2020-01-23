using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Portable;
using BodyArchitect.Shared;
using ICSharpCode.SharpZipLib.Zip;
using Ionic.Zlib;
using NUnit.Framework;
using Newtonsoft.Json;

namespace BodyArchitect.UnitTests.V2.Performance
{
    [DataContract]
    public class GPSPoint1
    {
        public GPSPoint1()
        {
        }

        public GPSPoint1(float latitude, float longitude, float altitude, float speed, DateTime dateTime)
        {
            Speed = speed;
            Latitude = latitude;
            Longitude = longitude;
            Altitude = altitude;
            DateTime = dateTime;
        }

        [DataMember]
        public float Speed { get; set; }

        [DataMember]
        public float Latitude { get; set; }

        [DataMember]
        public float Longitude { get; set; }

        [DataMember]
        public float Altitude { get; set; }

        [DataMember]
        public DateTime DateTime { get; set; }
    }

    [DataContract]
    public class GPSPoint2
    {
        public GPSPoint2()
        {
        }

        public GPSPoint2(float latitude, float longitude, float altitude, float speed, DateTime dateTime)
        {
            S = speed;
            L = latitude;
            O = longitude;
            A = altitude;
            D = dateTime;
        }

        [DataMember]
        public float S { get; set; }

        [DataMember]
        public float L { get; set; }

        [DataMember]
        public float O { get; set; }

        [DataMember]
        public float A { get; set; }

        [DataMember]
        public DateTime D { get; set; }
    }

    [TestFixture]
    public class TestsGPSSerialization
    {
        #region Serialization size
        #region SharpZipLib
        /*
        269438 (normal)
        27746 (zipped)

        270978
        27735

        270768
        27717
         */
        [Test]
        public void LongNamesJsonSerialization_NormalAndZip_SharpZipLib_BlockCopyMethod()
        {
            List<GPSPoint1> points = new List<GPSPoint1>();
            Random rand = new Random();
            for (int i = 0; i < 1000; i++)
            {
                points.Add(new GPSPoint1((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble(), DateTime.Now));
            }

            string output = JsonConvert.SerializeObject(points);
            byte[] bytes = new byte[output.Length * sizeof(char)];
            System.Buffer.BlockCopy(output.ToCharArray(), 0, bytes, 0, bytes.Length);

            for (int i = 1; i < bytes.Length; i+=2)
            {
                Assert.AreEqual(0,bytes[i]);
            }
            var length1 = bytes.Length;

            using (MemoryStream stream = new MemoryStream())
            {
                ZipOutputStream zipOutputStream = new ZipOutputStream(stream);
                zipOutputStream.SetLevel(9);

                // Write the data to the ZIP file              
                ZipEntry entry = new ZipEntry("name");
                zipOutputStream.PutNextEntry(entry);
                zipOutputStream.Write(bytes, 0, bytes.Length);

                // Finish the ZIP file  
                zipOutputStream.Finish();
                var length2 = zipOutputStream.Length;

            }
            
        }

        /*
        135584
        23944

        135377
        23884

        135565
        23939
*/
        [Test]
        public void LongNamesJsonSerialization_NormalAndZip_SharpZipLib_EncodingMethod()
        {
            List<GPSPoint1> points = new List<GPSPoint1>();
            Random rand = new Random();
            for (int i = 0; i < 1000; i++)
            {
                points.Add(new GPSPoint1((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble(), DateTime.Now));
            }

            string output = JsonConvert.SerializeObject(points);
            var bytes=UTF8Encoding.UTF8.GetBytes(output);

            var length1 = bytes.Length;

            using (MemoryStream stream = new MemoryStream())
            {
                ZipOutputStream zipOutputStream = new ZipOutputStream(stream);
                zipOutputStream.SetLevel(9);

                // Write the data to the ZIP file              
                ZipEntry entry = new ZipEntry("name");
                zipOutputStream.PutNextEntry(entry);
                zipOutputStream.Write(bytes, 0, bytes.Length);

                // Finish the ZIP file  
                zipOutputStream.Finish();
                var length2 = zipOutputStream.Length;

            }

        }
        /*  204992
            26890

            204964
            26845

            205244
            26961
         */
        [Test]
        public void ShortNamesJsonSerialization_NormalAndZip_SharpZipLib()
        {
            List<GPSPoint2> points = new List<GPSPoint2>();
            Random rand = new Random();
            for (int i = 0; i < 1000; i++)
            {
                points.Add(new GPSPoint2((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble(), DateTime.Now));
            }

            string output = JsonConvert.SerializeObject(points);
            byte[] bytes = new byte[output.Length * sizeof(char)];
            System.Buffer.BlockCopy(output.ToCharArray(), 0, bytes, 0, bytes.Length);

            var length1 = bytes.Length;
            
            using (MemoryStream stream = new MemoryStream())
            {
                ZipOutputStream zipOutputStream = new ZipOutputStream(stream);
                zipOutputStream.SetLevel(9);

                // Write the data to the ZIP file              
                ZipEntry entry = new ZipEntry("name");
                zipOutputStream.PutNextEntry(entry);
                zipOutputStream.Write(bytes, 0, bytes.Length);

                // Finish the ZIP file  
                zipOutputStream.Finish();
                var length2 = zipOutputStream.Length;

            }

        }
        #endregion

        #region DotNetZip

        /*271126
        27889

        271034
        27830

        270944
        27749*/
        [Test]
        public void LongNamesJsonSerialization_NormalAndZip_DotNetZip_BlockCopyMethod()
        {
            List<GPSPoint1> points = new List<GPSPoint1>();
            Random rand = new Random();
            for (int i = 0; i < 1000; i++)
            {
                points.Add(new GPSPoint1((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble(), DateTime.Now));
            }

            string output = JsonConvert.SerializeObject(points);
            byte[] bytes = new byte[output.Length * sizeof(char)];
            System.Buffer.BlockCopy(output.ToCharArray(), 0, bytes, 0, bytes.Length);

            for (int i = 1; i < bytes.Length; i += 2)
            {
                Assert.AreEqual(0, bytes[i]);
            }
            var length1 = bytes.Length;
            byte[] compressedData;
            using (MemoryStream str = new MemoryStream())
            {
                using (var zip = new Ionic.Zip.ZipFile())
                {
                    zip.CompressionLevel = CompressionLevel.BestCompression;
                    zip.AddEntry("README.txt", bytes);
                    zip.Save(str);
                }
                var r = str.Length;
                compressedData = str.ToArray();
            }

            //test for unzipping
            using (MemoryStream dest = new MemoryStream())
            {
                using (MemoryStream str = new MemoryStream(compressedData))
                {
                    var entry = Ionic.Zip.ZipFile.Read(str);
                    var firstEntry = entry.First();
                    firstEntry.Extract(dest);
                }
                byte[] endData=dest.ToArray();
                for (int i = 0; i < endData.Length; i++)
                {
                    Assert.AreEqual(bytes[i], endData[i]);
                }
            }
        }


        
        /*135529
23987

135529
24022

135453
24021*/
        [Test]
        public void LongNamesJsonSerialization_NormalAndZip_DotNetZip_EncodingMethod()
        {
            List<GPSPoint1> points = new List<GPSPoint1>();
            Random rand = new Random();
            for (int i = 0; i < 1000; i++)
            {
                points.Add(new GPSPoint1((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble(), DateTime.Now));
            }

            string output = JsonConvert.SerializeObject(points);
            var bytes = UTF8Encoding.UTF8.GetBytes(output);

            var length1 = bytes.Length;

            byte[] compressedData;
            using (MemoryStream str = new MemoryStream())
            {
                using (var zip = new Ionic.Zip.ZipFile())
                {
                    zip.CompressionLevel = CompressionLevel.BestCompression;
                    zip.AddEntry("README.txt", bytes);
                    zip.Save(str);
                }
                var r = str.Length;
                compressedData = str.ToArray();
            }

            //test for unzipping
            using (MemoryStream dest = new MemoryStream())
            {
                using (MemoryStream str = new MemoryStream(compressedData))
                {
                    var entry = Ionic.Zip.ZipFile.Read(str);
                    var firstEntry = entry.First();
                    firstEntry.Extract(dest);
                }
                byte[] endData = dest.ToArray();
                for (int i = 0; i < endData.Length; i++)
                {
                    Assert.AreEqual(bytes[i], endData[i]);
                }
            }

        }

        /*101871
23536*/
        [Test]
        public void ShortNamesJsonSerialization_NormalAndZip_DotNetZip_EncodingMethod()
        {
            List<GPSPoint2> points = new List<GPSPoint2>();
            Random rand = new Random();
            for (int i = 0; i < 1000; i++)
            {
                points.Add(new GPSPoint2((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble(), DateTime.Now));
            }

            string output = JsonConvert.SerializeObject(points);
            var bytes = UTF8Encoding.UTF8.GetBytes(output);

            var length1 = bytes.Length;

            byte[] compressedData;
            using (MemoryStream str = new MemoryStream())
            {
                using (var zip = new Ionic.Zip.ZipFile())
                {
                    zip.CompressionLevel = CompressionLevel.BestCompression;
                    zip.AddEntry("README.txt", bytes);
                    zip.Save(str);
                }
                var r = str.Length;
                compressedData = str.ToArray();
            }

        }


        [Test]
        public void LongNamesJsonSerialization_NormalAndZip_DotNetZip_EncodingMethod_PropertyNullable()
        {
            List<GPSPoint> points = new List<GPSPoint>();
            Random rand = new Random();
            for (int i = 0; i < 1000; i++)
            {
                points.Add(new GPSPoint((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble()));
            }
            var setttings = new JsonSerializerSettings();
            setttings.NullValueHandling = NullValueHandling.Ignore;
            setttings.MissingMemberHandling = MissingMemberHandling.Ignore;
            string output = JsonConvert.SerializeObject(points, setttings);
            var bytes = UTF8Encoding.UTF8.GetBytes(output);

            var length1 = bytes.Length;

            byte[] compressedData;
            using (MemoryStream str = new MemoryStream())
            {
                using (var zip = new Ionic.Zip.ZipFile())
                {
                    zip.CompressionLevel = CompressionLevel.BestCompression;
                    zip.AddEntry("README.txt", bytes);
                    zip.Save(str);
                }
                var r = str.Length;
                compressedData = str.ToArray();
            }

            //test for unzipping
            using (MemoryStream dest = new MemoryStream())
            {
                using (MemoryStream str = new MemoryStream(compressedData))
                {
                    var entry = Ionic.Zip.ZipFile.Read(str);
                    var firstEntry = entry.First();
                    firstEntry.Extract(dest);
                }
                byte[] endData = dest.ToArray();
                for (int i = 0; i < endData.Length; i++)
                {
                    Assert.AreEqual(bytes[i], endData[i]);
                }
            }

        }
        #endregion
        //197681
        //197532
        //197057
        [Test]
        public void LongNamesDataContractSerialization_Normal()
        {
            List<GPSPoint1> points = new List<GPSPoint1>();
            Random rand = new Random();
            for (int i = 0; i < 1000; i++)
            {
                points.Add(new GPSPoint1((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble(), DateTime.Now));
            }

            MemoryStream stream = new MemoryStream();
            DataContractSerializer serializer = new DataContractSerializer(typeof(List<GPSPoint1>));
            serializer.WriteObject(stream, points);
            var length1 = stream.Length;

            //stream.Seek(0, SeekOrigin.Begin);
            //StreamReader reader = new StreamReader(stream);
            //var test=reader.ReadToEnd();
        }

        #endregion

        #region Amount of GPS points

        byte[] serializeJson(List<GPSPoint1> points, bool shouldZip)
        {
            string output = JsonConvert.SerializeObject(points);
            var bytes = UTF8Encoding.UTF8.GetBytes(output);

            var length1 = bytes.Length;
            byte[] result = bytes;
            if (shouldZip)
            {
                using (MemoryStream str = new MemoryStream())
                {
                    using (var zip = new Ionic.Zip.ZipFile())
                    {
                        zip.CompressionLevel = CompressionLevel.BestCompression;
                        zip.AddEntry("README.txt", bytes);
                        zip.Save(str);
                    }
                    var r = str.Length;
                    result = str.ToArray();
                }
            }
            return result;
        }

        /*Every 4 seconds for 2 hours (1800 points):
          243860,42681,5.71
          242715,42524,5.7
         
          Every 4 seconds for 10 hours (9000 points):
          1218129,217968,5.58
         
          Every 4 seconds for 24 hours (21600 points):
          2924566,522913,5.59
         */
        [Test]
        public void Test()
        {
            int seconds = 4;
            TimeSpan howLong = TimeSpan.FromHours(24);
            double totalPoints = howLong.TotalSeconds/seconds;
            List<GPSPoint1> points = new List<GPSPoint1>();
            Random rand = new Random();
            for (int i = 0; i < totalPoints; i++)
            {
                points.Add(new GPSPoint1((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble(), DateTime.Now));
            }

            var withoutCompression = serializeJson(points, false);
            var withCompression = serializeJson(points, true);
            double ration = (double)withoutCompression.Length / (double)withCompression.Length;
        }
        #endregion
    }
}
