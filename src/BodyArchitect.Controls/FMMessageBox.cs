using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.Controls.Forms;
using BodyArchitect.Shared;
using DevExpress.XtraEditors;
using Microsoft.Practices.EnterpriseLibrary.Validation;


namespace BodyArchitect.Controls
{
    public static class FMMessageBox
    {
        public static void ShowValidationError(ValidationResults result)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var message in result)
            {
                builder.AppendLine(message.Message);
            }
            ShowError(builder.ToString());
        }

        public static DialogResult AskYesNo(string message,params object[] args)
        {
            return XtraMessageBox.Show(string.Format(message, args), Constants.ApplicationName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        public static DialogResult AskWarningYesNo(string message, params object[] args)
        {
            return XtraMessageBox.Show(string.Format(message, args), Constants.ApplicationName, MessageBoxButtons.YesNo, MessageBoxIcon.Warning,MessageBoxDefaultButton.Button2);
        }

        public static void ShowError(string message, params object[] args)
        {
            XtraMessageBox.Show(string.Format(message, args), Constants.ApplicationName, MessageBoxButtons.OK,MessageBoxIcon.Error);
        }

        public static void ShowInfo(string message, params object[] args)
        {
            XtraMessageBox.Show(string.Format(message, args), Constants.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void ShowError(BaseWindow parent,string message, params object[] args)
        {
            if (parent!=null)
            {
                parent.SynchronizationContext.Send(delegate
                                                       {
                                                           XtraMessageBox.Show(string.Format(message, args), Constants.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                       },null);
            }
            
        }
    }
}
