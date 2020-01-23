//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Security.Policy;
//using System.Text;
//using Microsoft.Win32;

//namespace BodyArchitect
//{
//    static class DeploymentUtilsWin32
//    {
//        /// <summary>
//        /// Uninstall the current version of the application.
//        /// </summary>
//        public static void UninstallMe()
//        {
//            string publicKeyToken = GetPublicKeyToken();
//            // Find Uninstall string in registry
//            string DisplayName = null;
//            string uninstallString = GetUninstallString(publicKeyToken,
//              out DisplayName);
//            if (uninstallString.Length <= 0)
//            {
//                return;
//            }

//            string runDLL32 = uninstallString.Substring(0, 12);
//            string args = uninstallString.Substring(13);

//            //start the uninstall; this will bring up the uninstall dialog
//            //  asking if it's ok
//            Process uninstallProcess = Process.Start(runDLL32, args);

//            //push the OK button
//            PushUninstallOKButton(DisplayName);
//        }

//        /// <summary>
//        /// Gets the public key token
//        /// for the current running (ClickOnce) application.
//        /// </summary>
//        /// <returns></returns>
//        public static string GetPublicKeyToken()
//        {
//            ApplicationSecurityInfo asi = new
//      ApplicationSecurityInfo(AppDomain.CurrentDomain.ActivationContext);
//            byte[] pk = asi.ApplicationId.PublicKeyToken;
//            StringBuilder pkt = new StringBuilder();
//            for (int i = 0; i < pk.GetLength(0); i++)
//                pkt.Append(String.Format("{0:x2}", pk[i]));
//            return pkt.ToString();
//        }

//        /// <summary>
//        /// Gets the uninstall string for the current ClickOnce app
//        /// from the Windows Registry.
//        /// </summary>
//        /// <param name="PublicKeyToken">The public key token of the app.</param>
//        /// <returns>The command line to execute that will
//        /// uninstall the app.</returns>
//        public static string GetUninstallString(string PublicKeyToken,
//          out string DisplayName)
//        {
//            string uninstallString = null;

//            //set up the string to search for
//            string searchString = "PublicKeyToken=" + PublicKeyToken;

//            //open the registry key and get the subkey names
//            RegistryKey uninstallKey =
//              Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall");
//            string[] appKeyNames = uninstallKey.GetSubKeyNames();

//            DisplayName = null;
//            bool found = false;

//            //search through the list for one with a match
//            foreach (string appKeyName in appKeyNames)
//            {
//                RegistryKey appKey = uninstallKey.OpenSubKey(appKeyName);
//                uninstallString = (string)appKey.GetValue("UninstallString");
//                DisplayName = (string)appKey.GetValue("DisplayName");
//                appKey.Close();
//                if (uninstallString.Contains(PublicKeyToken)
//                  && DisplayName == "TestCertExp_CSharp")
//                {
//                    found = true;
//                    break;
//                }
//            }

//            uninstallKey.Close();

//            if (found)
//                return uninstallString;
//            else
//                return string.Empty;
//        }

//        /// <summary>
//        /// Find and Push the OK button on the uninstall dialog.
//        /// </summary>
//        /// <param name="DisplayName">Display Name value from the registry</param>
//        private static void PushUninstallOKButton(string DisplayName)
//        {
//            bool success = false;

//            //Find the uninstall dialog.
//            IntPtr uninstallerWin = FindUninstallerWindow(DisplayName, out success);
//            IntPtr OKButton = IntPtr.Zero;

//            //If it found the window, look for the button.
//            if (success)
//                OKButton = FindUninstallerOKButton(uninstallerWin, out success);

//            //If it found the button, press it.
//            if (success)
//                DeploymentUtilsWin32.DoButtonClick(OKButton);
//        }

//        /// <summary>
//        /// Find the uninstall dialog.
//        /// </summary>
//        /// <param name="DisplayName">Display Name retrieved
//        /// from the registry.</param>
//        /// <param name="success">Whether the window was found or not.</param>
//        /// <returns>Pointer to the uninstall dialog.</returns>
//        private static IntPtr FindUninstallerWindow(string DisplayName,
//          out bool success)
//        {
//            //Max number of times to look for the window,
//            //used to let you out if there's a problem.
//            int i = 25;
//            DeploymentUtilsWin32 w32 = new DeploymentUtilsWin32();
//            IntPtr uninstallerWindow = IntPtr.Zero;
//            while (uninstallerWindow == IntPtr.Zero && i > 0)
//            {
//                uninstallerWindow = w32.SearchForTopLevelWindow(DisplayName +
//                  " Maintenance");
//                System.Threading.Thread.Sleep(500);
//                i--;
//            }

//            if (uninstallerWindow == IntPtr.Zero)
//                success = false;
//            else
//                success = true;

//            return uninstallerWindow;
//        }

//        /// Find the OK button on the uninstall dialog.
//        /// </summary>
//        /// <param name="UninstallerWindow">The pointer to
//        /// the Uninstall Dialog</param>
//        /// <param name="success">Whether it succeeded or not.</param>
//        /// <returns>A pointer to the OK button</returns>
//        private static IntPtr FindUninstallerOKButton(IntPtr UninstallerWindow, out bool success)
//        {
//            //max number of times to look for the button,
//            //lets you out if there's a problem
//            int i = 25;
//            DeploymentUtilsWin32 w32 = new DeploymentUtilsWin32();
//            IntPtr OKButton = IntPtr.Zero;

//            while (OKButton == IntPtr.Zero && i > 0)
//            {
//                OKButton = w32.SearchForChildWindow(UninstallerWindow, "&OK");
//                System.Threading.Thread.Sleep(500);
//                i--;
//            }

//            if (OKButton == IntPtr.Zero)
//                success = false;
//            else
//                success = true;

//            return OKButton;
//        }

//        //C#
//        public static void InstallNewVersion()
//        {
//            string url =@"http://localhost/NewVersion/TestCertExp_CSharp.application";
//            System.Diagnostics.Process.Start("iexplore.exe", url);
//            System.Windows.Forms.Application.Exit();
//            return;
//        }
//    }

//}
