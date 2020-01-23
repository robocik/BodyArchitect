using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using BodyArchitect.Controls.Cache;
using BodyArchitect.Logger;
using BodyArchitect.Service.Model;
using BodyArchitect.Settings;
using BodyArchitect.Settings.Model;
using BodyArchitect.Shared;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using System.Windows.Forms;
using BodyArchitect.Controls.Forms;
using BodyArchitect.Controls.Localization;
using System.Threading;
using BodyArchitect.Common;
using System.Globalization;
using Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WinForms;

namespace BodyArchitect.Controls
{
    public static class ControlHelper
    {
        public const string TutorialCreateProfile = "http://www.youtube.com/watch?v=kiEITIU557U&feature=player_embedded";
        public const string TutorialAll = "http://bodyarchitectonline.com/index.php/tutorial";
        public const string TutorialWorkoutLog = "http://www.youtube.com/watch?v=MUSFlPNTo2w";
        public const string TutorialUsers = "http://www.youtube.com/watch?v=bCSEkBGZLLE&feature=player_embedded";
        public const string TutorialWorkoutPlans = "http://www.youtube.com/watch?v=4WFSxrlAdvQ&feature=player_embedded";

        public static void OpenUrl(string url)
        {
            try
            {
                System.Diagnostics.Process.Start(url);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Default.Process(ex, ApplicationStrings.ErrorOpenWebBrowser, ErrorWindow.MessageBox);
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
            double kg = (double) weight;
            if (((WeightType)Settings1.Default.WeightType) == WeightType.Pounds)
            {
                kg = kg * 0.454;
            }
            return kg;
        }

        public static double ToSaveLength(this decimal length)
        {
            double cm = (double) length;
            if (((LengthType)Settings1.Default.LengthType) == LengthType.Inchs)
            {
                cm = cm *2.54f;
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
                ExceptionHandler.Default.Process(ex, ApplicationStrings.ErrorOldDataModification, ErrorWindow.MessageBox);
            }
            catch (ProfileDeletedException ex)
            {
                ExceptionHandler.Default.Process(ex, ApplicationStrings.ErrorCurrentProfileDeleted, ErrorWindow.MessageBox);
                UserContext.Logout(resetAutologon: false, skipLogoutOnServer: true);
            }
            catch (UserDeletedException ex)
            {
                ExceptionHandler.Default.Process(ex, ApplicationStrings.ErrorProfileDeleted, ErrorWindow.MessageBox);
            }
            catch (ValidationException ex)
            {
                FMMessageBox.ShowValidationError(ex.Results);
            }
            catch (EndpointNotFoundException ex)
            {
                ExceptionHandler.Default.Process(ex, ApplicationStrings.ErrorConnectionProblem, ErrorWindow.MessageBox);
                UserContext.Logout(resetAutologon: false, skipLogoutOnServer: true);

            }
            catch (TimeoutException ex)
            {
                ExceptionHandler.Default.Process(ex, ApplicationStrings.ErrorConnectionProblem, ErrorWindow.MessageBox);
                UserContext.Logout(resetAutologon: false, skipLogoutOnServer: true);
            }
            catch (DatabaseVersionException ex)
            {
                ExceptionHandler.Default.Process(ex, ApplicationStrings.ErrorOldVersionOfBodyArchitect,ErrorWindow.MessageBox);
                UserContext.Logout(resetAutologon: false, skipLogoutOnServer: true);
            }
            catch (MaintenanceException ex)
            {
                ExceptionHandler.Default.Process(ex, ApplicationStrings.ErrorMaintenanceMode, ErrorWindow.MessageBox);
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
            string[] orders = new string[]{ "EB", "PB", "TB", "GB", "MB", "KB", "B" };
            var max = (long)Math.Pow(scale, (orders.Length - 1));
            foreach (string order in orders)
            {
                if (bytes > max)
                    return string.Format("{0:##.##} {1}", Decimal.Divide(bytes, max), order);
                max /= scale;
            }
            return "0 B";
        }

        public static bool DoValidate(this ValidationProvider validationProvider,Control ctrl)
        {
            var ttt = validationProvider.GetPerformValidation(ctrl);
            if (ttt)
            {
                validationProvider.PerformValidation(ctrl);
            }
            foreach (Control control in ctrl.Controls)
            {
                DoValidate(validationProvider,control);
            }
            return validationProvider.IsValid;
        }

        public static object GetSelectedItem(this LookUpEdit lookUpEdit)
        {
            return lookUpEdit.Properties.GetDataSourceRowByKeyValue(lookUpEdit.EditValue);
        }

        public static T GetSelectedItem<T>(this LookUpEdit lookUpEdit)
        {
            return (T)lookUpEdit.Properties.GetDataSourceRowByKeyValue(lookUpEdit.EditValue);
        }

        public static MainWindow MainWindow { get; internal set; }

        public static  void EnsureThreadLocalized()
        {
            if (!string.IsNullOrEmpty(UserContext.Settings.GuiState.Language))
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(UserContext.Settings.GuiState.Language);
                Thread.CurrentThread.CurrentCulture = new CultureInfo(ApplicationStrings.SpecificCulture);
            }
        }

        public static void SelectValue(this ComboBoxEdit comboBox, object obj)
        {
            foreach (ComboBoxItem comboBoxItem in comboBox.Properties.Items)
            {
                if (object.Equals(comboBoxItem.Tag, obj))
                {
                    comboBox.SelectedItem = comboBoxItem;
                    return;
                }
            }
        }

        public static void SelectValueString(this ComboBoxEdit comboBox,string obj)
        {
            foreach (ComboBoxItem comboBoxItem in comboBox.Properties.Items)
            {
                if ((string.IsNullOrEmpty((string)comboBoxItem.Tag) && string.IsNullOrEmpty(obj)) || object.Equals(comboBoxItem.Tag, obj))
                {
                    comboBox.SelectedItem = comboBoxItem;
                    return;
                }
            }
        }

        public static void  AddItem(this ListView listView,string text,object tag)
        {
            ListViewItem item = new ListViewItem(text);
            item.Tag = tag;
            listView.Items.Add(item);
        }

        public static T GetParentControl<T>(this Control ctrl) where T : Control
        {
            while (ctrl.Parent != null)
            {
                T form = ctrl.Parent as T;
                if (form != null)
                {
                    return form;
                }
                ctrl = ctrl.Parent;
            }
            return null;
        }


        public static void AddSuperTip(ToolTipController toolTipController, Control control, string title, string text)
        {
            SuperToolTip tip = new SuperToolTip();
            tip.AllowHtmlText = DefaultBoolean.True;
            if (!string.IsNullOrEmpty(title))
            {
                tip.Items.AddTitle(title);
            }
            tip.Items.Add(text);
            toolTipController.SetSuperTip(control, tip);
        }

        public static void AddSuperTip(BaseControl control,string title,string text)
        {
            control.SuperTip = new SuperToolTip();
            control.SuperTip.AllowHtmlText = DefaultBoolean.True;
            if (!string.IsNullOrEmpty(title))
            {
                control.SuperTip.Items.AddTitle(title);
            }
            control.SuperTip.Items.Add(text);
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
                { 0.75, x => ApplicationStrings.DateTimeExtension_LessThanMinute },
                { 1.5, x => ApplicationStrings.DateTimeExtension_OneMinute },
                { 45, x => String.Format(ApplicationStrings.DateTimeExtension_XMinutes,Math.Round( Math.Abs(x.TotalMinutes))) },
                { 90, x => ApplicationStrings.DateTimeExtension_OneHour },
                { 60 * 24, x => String.Format(ApplicationStrings.DateTimeExtension_XHours, Math.Round(Math.Abs(x.TotalHours))) }, // 
                { 60 * 48, x => ApplicationStrings.DateTimeExtension_OneDay },
                { 60 * 24 * 30, x => String.Format(ApplicationStrings.DateTimeExtension_XDays, Math.Floor(Math.Abs(x.TotalDays))) },
                { 60 * 24 * 60, x => ApplicationStrings.DateTimeExtension_OneMonth },
                { 60 * 24 * 365, x => String.Format(ApplicationStrings.DateTimeExtension_XMonths, Math.Floor(Math.Abs(x.TotalDays / 30))) },
                { 60 * 24 * 365 * 2, x => ApplicationStrings.DateTimeExtension_OneYear },
                { Double.MaxValue,  x =>x.ToString() }
            };

            calendarDateMap = new Dictionary<double, Func<TimeSpan, string>>
            {
                { 60 * 24, x => ApplicationStrings.DateTimeExtension_Today },
                { 60 * 48, x => ApplicationStrings.DateTimeExtension_Yesterday },
                { 60 * 24 * 30, x => String.Format(ApplicationStrings.DateTimeExtension_XDaysAgo, Math.Floor(Math.Abs(x.TotalDays))) },
                { 60 * 24 * 60, x => ApplicationStrings.DateTimeExtension_OneMonthAgo },
                { 60 * 24 * 365, x => String.Format(ApplicationStrings.DateTimeExtension_XMonthsAgo, Math.Floor(Math.Abs(x.TotalDays / 30))) },
                { 60 * 24 * 365 * 2, x => ApplicationStrings.DateTimeExtension_OneYearAgo },
                { Double.MaxValue, x =>x.ToString() }
            };
        }

        public static string ToRelativeDate(this DateTime input)
        {
            if (UserContext.Settings.GuiState.ShowRelativeDates)
            {
                TimeSpan diff = DateTime.Now.Subtract(input);
                double totalMinutes = diff.TotalMinutes;

                string suffix = ApplicationStrings.DateTimeExtension_AgoPostfix;
                if (!suffix.StartsWith(" "))
                {
                    suffix = " "+suffix;
                }
                if (totalMinutes < 0.0)
                {
                    totalMinutes = Math.Abs(totalMinutes);
                    suffix = ApplicationStrings.DateTimeExtension_FromNowSuffix;
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
    }
}
