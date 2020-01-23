using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Portable.Exceptions;
using BodyArchitect.Shared;

namespace BodyArchitect.Client.UI
{
    public static class BAMessageBox
    {
        public static void ShowValidationError(ICollection<ValidationResult> result)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var message in result)
            {
                builder.AppendLine(message.Message);
            }
            ShowError(builder.ToString());
        }

        public static MessageBoxResult AskYesNo(string message, params object[] args)
        {
            return MessageBox.Show(string.Format(message, args), Constants.ApplicationName, MessageBoxButton.YesNo, MessageBoxImage.Question);
        }

        public static MessageBoxResult AskWarningYesNo(string message, params object[] args)
        {
            return MessageBox.Show(string.Format(message, args), Constants.ApplicationName, MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
        }

        public static void ShowError(string message, params object[] args)
        {
            MessageBox.Show(string.Format(message, args), Constants.ApplicationName, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void ShowInfo(string message, params object[] args)
        {
            MessageBox.Show(string.Format(message, args), Constants.ApplicationName, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void ShowError(BaseWindow parent, string message, params object[] args)
        {
            if (parent != null)
            {
                parent.SynchronizationContext.Send(delegate
                {
                    MessageBox.Show(string.Format(message, args), Constants.ApplicationName, MessageBoxButton.OK, MessageBoxImage.Error);
                }, null);
            }

        }

        public static void ShowErrorEx(string message, string caption, params object[] args)
        {
            MessageBox.Show(string.Format(message, args), caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
