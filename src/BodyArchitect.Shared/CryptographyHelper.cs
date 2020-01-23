using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace BodyArchitect.Shared
{
    public static class CryptographyHelper
    {
        public static string Encrypt(string input, byte[] key)
        {
            byte[] inputArray = UTF8Encoding.UTF8.GetBytes(input);
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = key;
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static string Decrypt(string input, byte[] key)
        {
            byte[] inputArray = Convert.FromBase64String(input);
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = key;
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);
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
                //var zipStream=zf.GetInputStream(entry);

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
                zipOutputStream.UseZip64 = UseZip64.Off;
                
                // Write the data to the ZIP file              
                ZipEntry entry = new ZipEntry("zip");
                zipOutputStream.PutNextEntry(entry);
                zipOutputStream.Write(compressedData, 0, compressedData.Length);

                // Finish the ZIP file  
                zipOutputStream.Finish();
                return stream.ToArray();
            }
        }

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

        public static string GetMD5HashFromFile(Stream stream)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(stream);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
