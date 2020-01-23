using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using BodyArchitect.Portable;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Logger=Microsoft.Practices.EnterpriseLibrary.Logging.Logger;

namespace BodyArchitect.Logger
{
    public static class Log
    {
        private static bool enableExceptionLog=true;
        private static bool enableStandardLog;


        public static bool EnableExceptionLog
        {
            get { return enableExceptionLog; }
            set { enableExceptionLog = value; }
        }

        public static bool EnableStandardLog
        {
            get { return enableStandardLog; }
            set { enableStandardLog = value; }
        }

        /// <summary>
        /// Writes message with Verbose serverity (debug information). This method uses parameters in message text (like in <see cref="string.Format(string,object[])"/> method)
        /// </summary>
        /// <param name="message">Message text to write</param>
        /// <param name="args">An Object array containing zero or more objects to format</param>
        public static void WriteVerbose(string message, params object[] args)
        {
            if (EnableStandardLog)
            {
                Write(message, TraceEventType.Verbose, null, args);
            }
        }

        public static void Write(Exception exception, Guid exceptionID,params object[] args)
        {
            Write(exception, exceptionID, "Exception", args);
        }

        static private string getOSVersion()
        {
            string strVersion = "Unknown";
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32S:
                    strVersion = "Windows 3.1";
                    break;
                case PlatformID.Win32Windows:
                    switch (Environment.OSVersion.Version.Minor)
                    {
                        case 0:
                            strVersion = "Windows 95";
                            break;
                        case 10:
                            if (Environment.OSVersion.Version.Revision.ToString() == "2222A")
                            {
                                strVersion = "Windows 98 Second Edition";
                            }
                            else
                            {
                                strVersion = "Windows 98";
                            }
                            break;
                        case 90:
                            strVersion = "Windows ME";
                            break;
                    }
                    break;
                case PlatformID.Win32NT:
                    switch (Environment.OSVersion.Version.Major)
                    {
                        case 3:
                            strVersion = "Windows NT 3.51";
                            break;
                        case 4:
                            strVersion = "Windows NT 4.0";
                            break;
                        case 5:
                            switch (Environment.OSVersion.Version.Minor)
                            {
                                case 0:
                                    strVersion = "Windows 2000";
                                    break;
                                case 1:
                                    strVersion = "Windows XP";
                                    break;
                                case 2:
                                    strVersion = "Windows 2003";
                                    break;
                            }
                            break;
                        case 6:
                            
                            switch (Environment.OSVersion.Version.Minor)
                            {
                                case 0:
                                    strVersion = "Windows Vista";
                                    break;
                                case 1:
                                    strVersion = "Windows 7/2008";
                                    break;
                                case 2:
                                    strVersion = "Windows 8/2012";
                                    break;
                            }
                            break;
                    }
                    break;
                case PlatformID.WinCE:
                    strVersion = "Windows CE";
                    break;
                case PlatformID.Unix:
                    strVersion = "Unix";
                    break;
            }

            if(!string.IsNullOrEmpty(Environment.OSVersion.ServicePack))
            {
                strVersion += " " + Environment.OSVersion.ServicePack;
            }
            return strVersion;
        }

        /// <summary>
        /// Writes exception data to the log
        /// </summary>
        /// <param name="exception">Exception instance to write to the log</param>
        /// <param name="exceptionID">Exception identifier</param>
        public static void Write(Exception exception, Guid exceptionID,  string category , params object[] args)
        {
            if (enableExceptionLog || category=="email")
            {
                try
                {

                    string versionString = getVersion();


                    LogEntry entry = new LogEntry();
                    entry.Categories.Add(category);
                    entry.Message = exception.Message;
                    entry.Severity = TraceEventType.Critical;
                    entry.RelatedActivityId = exceptionID;
                    entry.setExtendedProperty("ExceptionID", exceptionID.ToString());
                    entry.setExtendedProperty("OS Version", getOSVersion());
                    entry.setExtendedProperty("App Version", versionString);
                    entry.setExtendedProperty("Full exception", exception.GetExceptionString());

                    for (int index = 0; index < args.Length; index++)
                    {
                        var arg = args[index];
                        entry.setExtendedProperty("Arg " + index, arg != null ? arg.ToString() : "null");
                    }

                    Microsoft.Practices.EnterpriseLibrary.Logging.Logger.Write(entry);
                }
                catch
                {
                }
            }
        }

        static void setExtendedProperty(this LogEntry entry,string name,string value)
        {
            if(value!=null)
            {
                entry.ExtendedProperties.Add(name,value);
            }
        }

        private static string getVersion()
        {
            string versionString = Constants.Version;
            var assembly = Assembly.GetEntryAssembly();
            if(assembly!=null)
            {
                Version version = assembly.GetName().Version;
                versionString = version.ToString();
            }
            return versionString;
        }

        /// <summary>
        /// Writes message to the log with specific severity. This method uses parameters in message text (like in <see cref="string.Format(string,object[])"/> method)
        /// </summary>
        /// <param name="message">Message text to write</param>
        /// <param name="severity">Severity of this message</param>
        /// <param name="args">An Object array containing zero or more objects to format</param>
        public static void Write(string message, TraceEventType severity,string category, params object[] args)
        {
            try
            {
                string versionString = getVersion();
                LogEntry entry = new LogEntry();
                entry.Categories.Add(category);
                entry.Message = string.Format(message, args);
                entry.Severity = severity;
                entry.ExtendedProperties.Add("App Version", versionString);
                Microsoft.Practices.EnterpriseLibrary.Logging.Logger.Write(entry);
            }
            catch
            {
            }
        }

        public static void WriteInfo(string message, params object[] args)
        {
            if (enableStandardLog)
            {
                Write(message, TraceEventType.Information, "General", args);
            }
        }

        public static void WriteWarning(string message, params object[] args)
        {
            if (enableStandardLog)
            {
                Write(message, TraceEventType.Warning, "General", args);
            }
        }

        public static void WriteError(string message, params object[] args)
        {
            if (enableStandardLog)
            {
                Write(message, TraceEventType.Error, "General", args);
            }
        }
    }
}
