using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace BodyArchitect.WP7.Client.WCF
{
    public static class CryptographyHelper
    {
        public static string ToSHA1Hash(this string plainText)
        {

            // Convert plain text into a byte array.
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            // Because we support multiple hashing algorithms, we must define
            // hash object as a common (abstract) base class. We will specify the
            // actual hashing algorithm class later during object creation.
            HashAlgorithm hash = new SHA1Managed();


            // Compute hash value of our plain text with appended salt.
            byte[] hashBytes = hash.ComputeHash(plainTextBytes);


            // Convert result into a base64-encoded string.
            string hashValue = Convert.ToBase64String(hashBytes);

            // Return the result.
            return hashValue;
        }

        public static byte[] FromZip(this byte[] compressedData)
        {
            using (MemoryStream stream = new MemoryStream(compressedData))
                return FromZip(stream);
        }

        public static byte[] FromZip(this Stream stream)
        {
            using (MemoryStream dest = new MemoryStream())
            {
                ZipInputStream zipInputStream = new ZipInputStream(stream);

                zipInputStream.GetNextEntry();

                StreamUtils.Copy(zipInputStream, dest, new byte[4096]);

                return dest.ToArray();
            }
        }

        public static byte[] ToZip(this byte[] compressedData)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                ZipOutputStream zipOutputStream = new ZipOutputStream(stream);
                zipOutputStream.SetLevel(9);
                zipOutputStream.UseZip64=UseZip64.Off;
                // Write the data to the ZIP file              
                ZipEntry entry = new ZipEntry("zip");
                zipOutputStream.PutNextEntry(entry);
                zipOutputStream.Write(compressedData, 0, compressedData.Length);

                // Finish the ZIP file  
                zipOutputStream.Finish();
                return stream.ToArray();
            }
        }
    }
}
