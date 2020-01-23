using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Logger;
using BodyArchitect.Portable;
using BodyArchitect.Settings;
using BodyArchitect.Settings.Model;
using BodyArchitect.Service.V2.Model;


namespace BodyArchitect.Client.Common
{


    public static class Helper
    {
        public const string TutorialCreateProfile = "http://www.youtube.com/watch?v=kiEITIU557U&feature=player_embedded";
        public const string TutorialAll = "http://bodyarchitectonline.com/index.php/tutorial";
        public const string TutorialWorkoutLog = "http://www.youtube.com/watch?v=MUSFlPNTo2w";
        public const string TutorialUsers = "http://www.youtube.com/watch?v=bCSEkBGZLLE&feature=player_embedded";
        public const string TutorialWorkoutPlans = "http://www.youtube.com/watch?v=4WFSxrlAdvQ&feature=player_embedded";

        public static BitmapFrame Resize(Stream photoStream, int width, int height,BitmapScalingMode scalingMode)
        {
            var photoDecoder = BitmapDecoder.Create(photoStream,BitmapCreateOptions.PreservePixelFormat,BitmapCacheOption.None);
            var photo = photoDecoder.Frames[0];

            var group = new DrawingGroup();
            RenderOptions.SetBitmapScalingMode(group, scalingMode);
            group.Children.Add(new ImageDrawing(photo,new Rect(0, 0, width, height)));
            var targetVisual = new DrawingVisual();
            var targetContext = targetVisual.RenderOpen();
            targetContext.DrawDrawing(group);
            var target = new RenderTargetBitmap(
                width, height, 96, 96, PixelFormats.Default);
            targetContext.Close();
            target.Render(targetVisual);
            var targetFrame = BitmapFrame.Create(target);
            return targetFrame;
        }

        public static bool SafeStartsWith(this string text, string textToCheck)
        {
            return text != null && text.ToLower().StartsWith(textToCheck);
        }

        public static System.Windows.Media.Imaging.BitmapSource GetBitmapSource(this System.Drawing.Image _image)
        {
            System.Drawing.Bitmap bitmap =(Bitmap) _image;
            System.Windows.Media.Imaging.BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    bitmap.GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            return bitmapSource;
        }

        public static bool IsDesignMode
        {
            get
            {
                return LicenseManager.UsageMode == LicenseUsageMode.Designtime;
            }
        }

        public static Uri ToResourceUrl(this string imageName)
        {
            return new Uri(ToResourceString(imageName));
        }

        public static string ToResourceString(this string imageName)
        {
            return string.Format(@"pack://application:,,,/BodyArchitect.Client.Resources;component/Images/{0}",imageName);
        }

        public static string StartUpPath
        {
            get { return Path.GetDirectoryName(StartUpFile); }
        }

        public static string StartUpFile
        {
            get { return System.Reflection.Assembly.GetEntryAssembly().Location; }
        }

        
        public static void OpenUrl(string url)
        {
            try
            {
                try
                {
                    //this internal try catch is because of sometimes when the default browser is not registered correctly then this throws an excetpion.
                    //in this case we force to open this URL in IE
                    System.Diagnostics.Process.Start(url);
                }
                catch (Exception)
                {
                    System.Diagnostics.Process.Start(@"IExplore.exe", url);
                }
                
            }
            catch (Exception ex)
            {
                ExceptionHandler.Default.Process(ex, Strings.ErrorOpenWebBrowser, ErrorWindow.MessageBox);
            }
        }

        public static decimal ToDisplayWeight(this decimal kg)
        {
            return (decimal)ToDisplayWeight((double)kg);
        }

        public static decimal ToDisplayLength(this decimal cm)
        {
            return (decimal) ToDisplayLength( (double)cm);
        }

        public static decimal ToDisplayTemperature(this decimal tempCelc)
        {
            if (UserContext.Current.ProfileInformation.Settings.LengthType == LengthType.Inchs)
            {
                tempCelc = ((9.0m / 5.0m) * tempCelc) + 32;
            }
            return tempCelc;
        }

        public static decimal FromDisplayTemperature(this decimal tempDisp)
        {
            if (UserContext.Current.ProfileInformation.Settings.LengthType == LengthType.Inchs)
            {
                tempDisp = (5.0m / 9.0m) * (tempDisp - 32);
            }
            return tempDisp;
        }



        public static TimeSpan ToDisplayDuration(this decimal seconds)
        {
            return TimeSpan.FromSeconds((int)seconds);
        }

        public static decimal FromDisplayAltitude(this decimal distance)
        {
            if (UserContext.Current.ProfileInformation.Settings.LengthType == Service.V2.Model.LengthType.Inchs)
            {
                distance = distance/3.2808m;
            }
            return distance;//to meters
        }

        public static decimal FromDisplayDistance(this decimal distance)
        {
            if (UserContext.Current.ProfileInformation.Settings.LengthType == Service.V2.Model.LengthType.Inchs)
            {
                distance = distance * 1.609344m;//to km
            }
            return distance * 1000;//to meters
        }

        public static decimal ToDisplayDistance(this decimal m)
        {
            var km = m / 1000;//to kilometers
            if (UserContext.Current.ProfileInformation.Settings.LengthType == LengthType.Inchs)
            {
                km = km * 0.621371m;
            }
            return km;
        }

        public static decimal ToDisplayAltitude(this decimal meters)
        {
            if (UserContext.Current.ProfileInformation.Settings.LengthType == LengthType.Inchs)
            {
                meters = meters * 3.2808m;
            }
            return meters;
        }

        public static decimal ToDisplayPace(this decimal mPerSec)
        {
            return (decimal) WilksFormula.SpeedToPace((float) mPerSec,UserContext.Current.ProfileInformation.Settings.LengthType ==LengthType.Inchs);
        }

        public static decimal ToDisplaySpeed(this decimal mPerSec)
        {
            var kmPerHour = mPerSec * 3.6m;
            if (UserContext.Current.ProfileInformation.Settings.LengthType ==LengthType.Inchs)
            {
                kmPerHour = kmPerHour * 0.621371m;
            }
            return kmPerHour;
        }

        public static double ToDisplayWeight(this double kg)
        {

            if (UserContext.Current.ProfileInformation.Settings.WeightType == WeightType.Pounds)
            {
                kg = kg / 0.454f;
            }
            return kg;
        }


        public static double ToDisplayLength(this double cm)
        {
            if (UserContext.Current.ProfileInformation.Settings.LengthType == LengthType.Inchs)
            {
                cm = cm / 2.54f;
            }
            return cm;
        }

        public static decimal ToSaveWeight(this double? weight)
        {
            if(weight==null)
            {
                weight = 0;
            }
            decimal kg = (decimal)weight;
            if (UserContext.Current.ProfileInformation.Settings.WeightType == WeightType.Pounds)
            {
                kg = kg * 0.454M;
            }
            return kg;
        }

        public static decimal ToSaveLength(this double? length)
        {
            if (length == null)
            {
                length = 0;
            }
            decimal cm = (decimal)length;
            if (UserContext.Current.ProfileInformation.Settings.LengthType == LengthType.Inchs)
            {
                cm = cm * 2.54M;
            }
            return cm;
        }

        [Conditional("DEBUG")] 
        public static void Delay(int miliseconds)
        {
            Thread.Sleep(miliseconds);
        }

        
        public static string FormatBytes(long bytes)
        {
            const long scale = 1024;
            string[] orders = new string[] { "EB", "PB", "TB", "GB", "MB", "KB", "B" };
            var max = (long)Math.Pow(scale, (orders.Length - 1));
            foreach (string order in orders)
            {
                if (bytes > max)
                    return string.Format("{0:##.##} {1}", Decimal.Divide(bytes, max), order);
                max /= scale;
            }
            return "0 B";
        }

        public static void EnsureThreadLocalized()
        {
            if (!string.IsNullOrEmpty(UserContext.Current.Settings.GuiState.Language))
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(UserContext.Current.Settings.GuiState.Language);
                Thread.CurrentThread.CurrentCulture = new CultureInfo(Strings.SpecificCulture);
            }
        }
    }

    public static class DateTimeExtension
    {
        static readonly Dictionary<double, Func<TimeSpan, string>> relativeDateMap;

        static readonly Dictionary<double, Func<TimeSpan, string>> calendarDateMap;
        
        public static bool IsFuture(this DateTime date)
        {
            return date > DateTime.Now.Date;
        }

        static DateTimeExtension()
        {
            relativeDateMap = new Dictionary<double, Func<TimeSpan, string>>
            {
                { 0.75, x => Strings.DateTimeExtension_LessThanMinute },
                { 1.5, x => Strings.DateTimeExtension_OneMinute },
                { 45, x => String.Format(Strings.DateTimeExtension_XMinutes,Math.Round( Math.Abs(x.TotalMinutes))) },
                { 90, x => Strings.DateTimeExtension_OneHour },
                { 60 * 24, x => String.Format(Strings.DateTimeExtension_XHours, Math.Round(Math.Abs(x.TotalHours))) }, // 
                { 60 * 48, x => Strings.DateTimeExtension_OneDay },
                { 60 * 24 * 30, x => String.Format(Strings.DateTimeExtension_XDays, Math.Floor(Math.Abs(x.TotalDays))) },
                { 60 * 24 * 60, x => Strings.DateTimeExtension_OneMonth },
                { 60 * 24 * 365, x => String.Format(Strings.DateTimeExtension_XMonths, Math.Floor(Math.Abs(x.TotalDays / 30))) },
                { 60 * 24 * 365 * 2, x => Strings.DateTimeExtension_OneYear },
                { Double.MaxValue,  x =>x.ToString() }
            };

            calendarDateMap = new Dictionary<double, Func<TimeSpan, string>>
            {
                { 60 * 24, x => Strings.DateTimeExtension_Today },
                { 60 * 48, x => Strings.DateTimeExtension_Yesterday },
                { 60 * 24 * 30, x => String.Format(Strings.DateTimeExtension_XDaysAgo, Math.Floor(Math.Abs(x.TotalDays))) },
                { 60 * 24 * 60, x => Strings.DateTimeExtension_OneMonthAgo },
                { 60 * 24 * 365, x => String.Format(Strings.DateTimeExtension_XMonthsAgo, Math.Floor(Math.Abs(x.TotalDays / 30))) },
                { 60 * 24 * 365 * 2, x => Strings.DateTimeExtension_OneYearAgo },
                { Double.MaxValue, x =>x.ToString() }
            };
        }

        public static string ToRelativeDate(this DateTime input)
        {
            if (UserContext.Current.Settings.GuiState.ShowRelativeDates)
            {
                TimeSpan diff = DateTime.Now.Subtract(input);
                if (diff > TimeSpan.FromDays(365*2))
                {
                    return input.ToString();
                }
                double totalMinutes = diff.TotalMinutes;

                string suffix = Strings.DateTimeExtension_AgoPostfix;
                
                if (totalMinutes < 0.0)
                {
                    totalMinutes = Math.Abs(totalMinutes);
                    suffix = Strings.DateTimeExtension_FromNowSuffix;
                }
                if (!suffix.StartsWith(" "))
                {
                    suffix = " " + suffix;
                }
                return relativeDateMap.First(n => totalMinutes < n.Key).Value(diff) + suffix;
            }
            return input.ToString();
        }

        public static string ToCalendarDate(this DateTime input)
        {
            if (UserContext.Current.Settings.GuiState.ShowRelativeDates)
            {
                TimeSpan diff = DateTime.Now.Date.Subtract(input.Date);
                double totalMinutes = diff.TotalMinutes;
                return calendarDateMap.First(n => totalMinutes < n.Key).Value(diff);
            }
            return input.ToShortDateString();
        }

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
