using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using BodyArchitect.Controls.Forms;
using BodyArchitect.Logger;
using BodyArchitect.Controls;

using BodyArchitect.Controls.Localization;


namespace BodyArchitect
{
    static class Program
    {
        
        /// <summary>InterfaceType
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            ExceptionHandler.Default.ShowEmailReportWindow += new EventHandler<ErrorEventArgs>(Default_ShowEmailReportWindow);
            ExceptionHandler.Default.ShowMessageBoxWindow += new EventHandler<ErrorEventArgs>(Default_ShowMessageBoxWindow);
            ControlHelper.EnsureThreadLocalized();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            SingleInstanceController controller = null;
            try
            {
                controller = new SingleInstanceController();
                controller.Run(args);
            }
            catch (Exception ex)
            {
                if (controller!=null)
                {
                    controller.HideSplashScreen();
                }
                ExceptionHandler.Default.Process(ex, ApplicationStrings.ErrorUnhandledException, ErrorWindow.EMailReport);
            }
        }

        

        static void Default_ShowMessageBoxWindow(object sender, ErrorEventArgs e)
        {
            FMMessageBox.ShowError(e.DisplayMessage, e.ErrorId);
        }

        static void Default_ShowEmailReportWindow(object sender, ErrorEventArgs e)
        {
            SendErrorWindow dlg = new SendErrorWindow();
            dlg.Fill(e.Exception);
            e.ShouldSend = dlg.ShowDialog() == DialogResult.OK;
            e.ApplyAlways = dlg.ApplyAlways;
        }
    }
}
