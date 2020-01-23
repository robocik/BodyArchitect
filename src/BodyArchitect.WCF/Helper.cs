using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Logger;
using BodyArchitect.Settings;
using BodyArchitect.Settings.Model;
using BodyArchitect.Shared;


namespace BodyArchitect.Client.WCF
{
    public static class Helper
    {
        public const string TutorialCreateProfile = "http://www.youtube.com/watch?v=kiEITIU557U&feature=player_embedded";
        public const string TutorialAll = "http://bodyarchitectonline.com/index.php/tutorial";
        public const string TutorialWorkoutLog = "http://www.youtube.com/watch?v=MUSFlPNTo2w";
        public const string TutorialUsers = "http://www.youtube.com/watch?v=bCSEkBGZLLE&feature=player_embedded";
        public const string TutorialWorkoutPlans = "http://www.youtube.com/watch?v=4WFSxrlAdvQ&feature=player_embedded";

        public static bool IsDesignMode
        {
            get
            {
                return LicenseManager.UsageMode == LicenseUsageMode.Designtime;
            }
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
                System.Diagnostics.Process.Start(url);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Default.Process(ex, Strings.ErrorOpenWebBrowser, ErrorWindow.MessageBox);
            }
        }

        public static double ToDisplayWeight(this float kg)
        {
            if (((WeightType)Settings1.Default.WeightType) == WeightType.Pounds)
            {
                kg = kg / 0.454f;
            }
            return kg;
        }

        public static double ToDisplayLength(this float cm)
        {
            if (((LengthType)Settings1.Default.LengthType) == LengthType.Inchs)
            {
                cm = cm / 2.54f;
            }
            return cm;
        }

        public static double ToSaveWeight(this decimal weight)
        {
            double kg = (double)weight;
            if (((WeightType)Settings1.Default.WeightType) == WeightType.Pounds)
            {
                kg = kg * 0.454;
            }
            return kg;
        }

        public static double ToSaveLength(this decimal length)
        {
            double cm = (double)length;
            if (((LengthType)Settings1.Default.LengthType) == LengthType.Inchs)
            {
                cm = cm * 2.54f;
            }
            return cm;
        }

        public static bool RunWithExceptionHandling(Action action)
        {
            try
            {
                action();
                return true;
            }
            catch (OperationCanceledException)
            {

            }
            catch (OldDataException ex)
            {
                ExceptionHandler.Default.Process(ex, Strings.ErrorOldDataModification, ErrorWindow.MessageBox);
            }
            catch (ProfileDeletedException ex)
            {
                ExceptionHandler.Default.Process(ex, Strings.ErrorCurrentProfileDeleted, ErrorWindow.MessageBox);
                UserContext.Logout(resetAutologon: false, skipLogoutOnServer: true);
            }
            catch (UserDeletedException ex)
            {
                ExceptionHandler.Default.Process(ex, Strings.ErrorProfileDeleted, ErrorWindow.MessageBox);
            }
            catch (ValidationException ex)
            {
                //FMMessageBox.ShowValidationError(ex.Results);
            }
            catch (EndpointNotFoundException ex)
            {
                ExceptionHandler.Default.Process(ex, Strings.ErrorConnectionProblem, ErrorWindow.MessageBox);
                UserContext.Logout(resetAutologon: false, skipLogoutOnServer: true);

            }
            catch (TimeoutException ex)
            {
                ExceptionHandler.Default.Process(ex, Strings.ErrorConnectionProblem, ErrorWindow.MessageBox);
                UserContext.Logout(resetAutologon: false, skipLogoutOnServer: true);
            }
            catch (DatabaseVersionException ex)
            {
                ExceptionHandler.Default.Process(ex, Strings.ErrorOldVersionOfBodyArchitect, ErrorWindow.MessageBox);
                UserContext.Logout(resetAutologon: false, skipLogoutOnServer: true);
            }
            catch (MaintenanceException ex)
            {
                ExceptionHandler.Default.Process(ex, Strings.ErrorMaintenanceMode, ErrorWindow.MessageBox);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Default.Process(ex, ex.Message, ErrorWindow.EMailReport);
            }
            return false;
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

        
    }

    public static class DateTimeExtension
    {
        static readonly Dictionary<double, Func<TimeSpan, string>> relativeDateMap;

        static readonly Dictionary<double, Func<TimeSpan, string>> calendarDateMap;

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
            if (UserContext.Settings.GuiState.ShowRelativeDates)
            {
                TimeSpan diff = DateTime.Now.Subtract(input);
                double totalMinutes = diff.TotalMinutes;

                string suffix = Strings.DateTimeExtension_AgoPostfix;
                if (!suffix.StartsWith(" "))
                {
                    suffix = " " + suffix;
                }
                if (totalMinutes < 0.0)
                {
                    totalMinutes = Math.Abs(totalMinutes);
                    suffix = Strings.DateTimeExtension_FromNowSuffix;
                }

                return relativeDateMap.First(n => totalMinutes < n.Key).Value(diff) + suffix;
            }
            return input.ToString();
        }

        public static string ToCalendarDate(this DateTime input)
        {
            if (UserContext.Settings.GuiState.ShowRelativeDates)
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
