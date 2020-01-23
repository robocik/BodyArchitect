using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Logger;
using Microsoft.Win32;

namespace BodyArchitect.Client.UI.Views
{
    [Serializable]
    public class LogPageContext:PageContext
    {
        private bool errorLog;

        public LogPageContext( bool errorLog)
        {
            this.errorLog = errorLog;
        }

        public LogPageContext()
        {
            
        }


        public bool ErrorLog
        {
            get { return errorLog; }
            set { errorLog = value; }
        }
    }
    /// <summary>
    /// Interaction logic for LogView.xaml
    /// </summary>
    public partial class LogView
    {

        private bool turnOn;

        public bool TurnOnLog
        {
            get { return turnOn; }
            set
            {
                turnOn = value;
                saveSettings();
                NotifyOfPropertyChange(()=>TurnOnLog);
            }
        }

        public LogPageContext LogPageContext
        {
            get { return (LogPageContext) PageContext; }
        }

        void saveSettings()
        {
            if (LogPageContext.ErrorLog)
            {
                UserContext.Current.Settings.LogErrorEnabled = TurnOnLog;
            }
            else
            {
                UserContext.Current.Settings.LogStandardEnabled = TurnOnLog;
            }
            UserContext.Current.Settings.Save();
        }
        
        public LogView()
        {
            InitializeComponent();
            DataContext = this;
        }

        private string Filename
        {
            get
            {
                string filename = UserContext.Current.Settings.ExceptionsLogFile;
                if (LogPageContext.ErrorLog)
                {
                    filename = UserContext.Current.Settings.ExceptionsLogFile;
                }
                else
                {
                    filename = UserContext.Current.Settings.StandardLogFile;
                }
                return filename;
            }
        }



        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = Strings.SaveLogFileFilter;
            if (dlg.ShowDialog() == true)
            {
                try
                {
                    File.WriteAllText(dlg.FileName, txtLogContent.Text);
                }
                catch (Exception ex)
                {
                    ExceptionHandler.Default.Process(ex, Strings.ErrorSaveLogFile, ErrorWindow.MessageBox);
                }
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            if(BAMessageBox.AskYesNo(Strings.QAskClearLog)==MessageBoxResult.Yes)
            {
                try
                {
                    string filename = UserContext.Current.Settings.ExceptionsLogFile;
                    if (LogPageContext.ErrorLog)
                    {
                        filename = UserContext.Current.Settings.ExceptionsLogFile;
                    }
                    else
                    {
                        filename = UserContext.Current.Settings.StandardLogFile;
                    }
                    if (File.Exists(filename))
                    {
                        File.Delete(filename);
                    }
                    txtLogContent.Text = string.Empty;
                }
                catch (Exception ex)
                {
                    ExceptionHandler.Default.Process(ex, Strings.ErrorDeleteLogFile, ErrorWindow.MessageBox);
                }
                

            }
        
        }


        public override void Fill()
        {
            Header = LogPageContext.ErrorLog ? EnumLocalizer.Default.GetStringsString("LogView_Fill_Header_Exceptions") : EnumLocalizer.Default.GetStringsString("LogView_Fill_Header_Standard");
            TurnOnLog = LogPageContext.ErrorLog ? UserContext.Current.Settings.LogErrorEnabled : UserContext.Current.Settings.LogStandardEnabled;
            string file = Filename;
            ParentWindow.RunAsynchronousOperation(delegate
                                                      {
                                                          if (File.Exists(file))
                                                          {
                                                              using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, System.IO.FileShare.ReadWrite))
                                                              {
                                                                  StreamReader reader = new StreamReader(fs);
                                                                  string fileContent=reader.ReadToEnd();
                                                                  
                                                                  Dispatcher.BeginInvoke(new Action(delegate
                                                                                                   {
                                                                                                       txtLogContent.Text = fileContent;
                                                                                                   }));
                                                                  
                                                              }
                                                          }
                                                      });
            
        }

        public override void RefreshView()
        {
            Fill();
        }



        public override Uri HeaderIcon
        {
            get
            {
                if (LogPageContext != null)
                {
                    string icon = LogPageContext.ErrorLog ? "LogError.png" : "LogStandard.png";
                    return icon.ToResourceUrl();
                }
                return null;
            }
        }
    }
}
