using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.IO;
using BodyArchitect.Common;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Logger;

namespace BodyArchitect.Controls.Forms
{
    public partial class LogViewer : DevExpress.XtraEditors.XtraForm
    {
        private bool errorLog;

        public LogViewer(string logFile, bool errorLog)
        {
            this.errorLog = errorLog;
            InitializeComponent();
            TurnOnLog = errorLog ? UserContext.Settings.LogErrorEnabled : UserContext.Settings.LogStandardEnabled;
            if (File.Exists(logFile))
            {
                using (FileStream fs = new FileStream(logFile, FileMode.Open, FileAccess.Read, System.IO.FileShare.ReadWrite))
                {
                    StreamReader reader = new StreamReader(fs);

                    //memoEdit1.Text = File.ReadAllText(logFile);
                    memoEdit1.Text = reader.ReadToEnd();
                }
            }
        }

        public bool TurnOnLog
        {
            get { return chkLoggingEnabled.Checked; }
            set { chkLoggingEnabled.Checked = value; }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = ApplicationStrings.SaveLogFileFilter;
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    File.WriteAllText(dlg.FileName, memoEdit1.Text);
                }
                catch (Exception ex)
                {
                    ExceptionHandler.Default.Process(ex, ApplicationStrings.ErrorSaveLogFile, ErrorWindow.MessageBox);
                }
            }
        }

        private void LogViewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (errorLog)
            {
                UserContext.Settings.LogErrorEnabled = TurnOnLog;
            }
            else
            {
                UserContext.Settings.LogStandardEnabled = TurnOnLog;
            }
            UserContext.Settings.Save();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if(FMMessageBox.AskYesNo(ApplicationStrings.QAskClearLog)==DialogResult.Yes)
            {
                try
                {
                    string filename = UserContext.Settings.ExceptionsLogFile;
                    if (errorLog)
                    {
                        filename = UserContext.Settings.ExceptionsLogFile;
                    }
                    else
                    {
                        filename = UserContext.Settings.StandardLogFile;
                    }
                    if (File.Exists(filename))
                    {
                        File.Delete(filename);
                    }
                    memoEdit1.Text = string.Empty;
                }
                catch (Exception ex)
                {
                    ExceptionHandler.Default.Process(ex, ApplicationStrings.ErrorDeleteLogFile, ErrorWindow.MessageBox);
                }
                

            }
        }
    }
}