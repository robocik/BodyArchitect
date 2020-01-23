using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;


namespace BodyArchitect.Common
{
    public static class Helper
    {
        public static Stream GetResource(string resourceName)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            Stream stream = assembly.GetManifestResourceStream(resourceName);
            return stream;
        }

        public static Image ToImage(byte[] imageData)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                memoryStream.Write(imageData, 0, imageData.Length);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return Image.FromStream(memoryStream);
            }
        }

        public static bool IsDesignMode
        {
            get
            {
                return LicenseManager.UsageMode == LicenseUsageMode.Designtime;
            }
        }

        public static byte[] ToByteArray(Image image)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                image.Save(memoryStream,ImageFormat.Png);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return memoryStream.ToArray();
            }
        }

        public static Image ResizeImage(byte[] imageData, int NewWidth, int MaxHeight, bool OnlyResizeIfWider)
        {
            return ResizeImage(ToImage(imageData), NewWidth, MaxHeight, OnlyResizeIfWider);
        }

        public static Image ResizeImage(Image originalImage,int NewWidth, int MaxHeight, bool OnlyResizeIfWider)
        {
            
            // Prevent using images internal thumbnail
            originalImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);
            originalImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);

            if (OnlyResizeIfWider)
            {
                if (originalImage.Width <= NewWidth)
                {
                    NewWidth = originalImage.Width;
                }
            }

            int NewHeight = originalImage.Height * NewWidth / originalImage.Width;
            if (NewHeight > MaxHeight)
            {
                // Resize with height instead
                NewWidth = originalImage.Width * MaxHeight / originalImage.Height;
                NewHeight = MaxHeight;
            }

            System.Drawing.Image NewImage = originalImage.GetThumbnailImage(NewWidth, NewHeight, null, IntPtr.Zero);
            
            return NewImage;
        }

        //public static string GetFullMessage(this ErrorSummary summary)
        //{
        //    StringBuilder builder = new StringBuilder();
        //    for (int i = 0; i < summary.ErrorsCount; i++)
        //    {
        //        builder.AppendFormat("{0}:{1}\r\n", summary.InvalidProperties[i], summary.ErrorMessages[i]);
        //    }
        //    return builder.ToString();
        //}
    }
}
