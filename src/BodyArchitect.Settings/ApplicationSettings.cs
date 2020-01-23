using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Deployment.Application;
using System.ServiceModel;

namespace BodyArchitect.Settings
{
    public class ApplicationSettings
    {
        GuiStateSettings gui = new GuiStateSettings();
        static List<ApplicationSettingsBase> optionList = new List<ApplicationSettingsBase>();

        static ApplicationSettings()
        {
            Register(Settings1.Default);
            Register(Settings.GuiState.Default);
        }

        static public void Upgrade()
        {
            if (ApplicationDeployment.IsNetworkDeployed && ApplicationDeployment.CurrentDeployment.IsFirstRun)
            {
                foreach (var options in optionList)
                {
                    options.Upgrade();
                }
            }
        }

        public void Reset()
        {
            foreach (var options in optionList)
            {
                options.Reset();
            }
        }


        public void SetProfileConfigurationWizardShowed(int profileId, bool showed)
        {
            if (Settings1.Default.ProfileConfigWizardStatus == null)
            {
                Settings1.Default.ProfileConfigWizardStatus = new Hashtable();
            }
            Settings1.Default.ProfileConfigWizardStatus[profileId] = showed;
        }

        public bool GetProfileConfigurationWizardShowed(Guid profileId)
        {
            if (Settings1.Default.ProfileConfigWizardStatus != null && Settings1.Default.ProfileConfigWizardStatus.ContainsKey(profileId))
            {
                return (bool)Settings1.Default.ProfileConfigWizardStatus[profileId];
            }
            return false;
        }

        public string DataPath
        {
            get
            {
                string path;
                if(ConfigurationManager.AppSettings["DataPath"]!=null)
                {
                    path= ConfigurationManager.AppSettings["DataPath"];
                }
                else
                {
                    path=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BodyArchitect");
                }
                if(!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }
        }

        public bool SendRequests
        {
            get
            {
                try
                {
                    if (ConfigurationManager.AppSettings["SendRequest"] != null)
                    {
                        return bool.Parse(ConfigurationManager.AppSettings["SendRequest"]);
                    }
                }
                catch (Exception)
                {
                }

                return true;
            }
        }

        public string ExceptionsLogFile
        {
            get
            {
                return Path.Combine(DataPath, "logs\\exceptions.log");
            }
        }

        public string StandardLogFile
        {
            get
            {
                return Path.Combine(DataPath, "logs\\test_log.log");
            }
        }

        public static bool AskForSendingException
        {
            get
            {
                return Settings1.Default.AskForSendingExceptions;
            }
            set
            {
                Settings1.Default.AskForSendingExceptions = value;
            }
        }

        public static bool SendExceptionsWithoutQuestion
        {
            get
            {
                return Settings1.Default.SendExceptionsWithoutQuestion;   
            }
            set
            {
                Settings1.Default.SendExceptionsWithoutQuestion = value;
            }
        }
        public static string MailSmtp
        {
            get
            {
                return "smtp.gmail.com";
            }
        }

        public static string MailUserName
        {
            get
            {
                return "baerror";
            }
        }

        public static string MailPassword
        {
            get
            {
                return "BAerR0r5_3cO*T@";
            }
        }

        public Guid ClientInstanceId
        {
            get
            {
                if (Settings1.Default.ClientInstanceId == Guid.Empty)
                {
                    Settings1.Default.ClientInstanceId = Guid.NewGuid();
                }
                return Settings1.Default.ClientInstanceId;
            }
        }

        public static string Server
        {
            get { return ServerUrl; }
        }

        public static string MailAccount
        {
            get { return "baerror@email.com"; }
        }

        public static string ServerUrl { get; set; }

        public static string TermsOfServiceAddress
        {
            get
            {
                return ServerUrl+"TermsOfUse.htm";
            }
        }

        public static string PrivacyPolicyAddress
        {
            get
            {
                return ServerUrl+"PrivacyPolicy.htm";
            }
        }

        public string RssNewsChannelAddress
        {
            get
            {
                return ServerUrl+"Rss/ba{0}.rss";
            }
        }

        public string RssTipsChannelAddress
        {
            get
            {
                //return ServerUrl+"Rss/tips{0}.rss";
                return "http://service.bodyarchitectonline.com/Rss/tips{0}.rss";
            }
        }

        public string BaUpdateCheckAddress
        {
            get
            {
                return "http://update.bodyarchitectonline.com/check.php?ClientID={0}&Lang={1}&Version={2}";
            }
        }

        public string ClickOnceUrl
        {
            get
            {
                return "http://update.bodyarchitectonline.com/BodyArchitect.application";
            }
        }

        public string WWW1
        {
            get
            {
                return "http://bodyarchitectonline.com";
            }
        }

        public GuiStateSettings GuiState
        {
            get
            {
                return gui;
            }
        }


        public int SerieNumberComboBoxSelectedItem
        {
            get
            {
                return Settings1.Default.SerieNumberComboBoxSelectedItem;
            }
            set
            {
                Settings1.Default.SerieNumberComboBoxSelectedItem = value;
            }
        }

        public bool LogErrorEnabled
        {
            get
            {
                return Settings1.Default.LogErrorEnabled;
            }
            set
            {
                Settings1.Default.LogErrorEnabled = value;
            }
        }

        public bool LogStandardEnabled
        {
            get
            {
                return Settings1.Default.LogStandardEnabled;
            }
            set
            {
                Settings1.Default.LogStandardEnabled = value;
            }
        }

        public int ExerciseComboBoxSelectedItem
        {
            get
            {
                return Settings.Settings1.Default.ExerciseComboBoxSelectedItem;
            }
            set
            {
                Settings1.Default.ExerciseComboBoxSelectedItem = value;
            }
        }
        
        public void Save()
        {
            foreach (var options in optionList)
            {
                options.Save();
            }
            //Settings1.Default.Save();
            //gui.Save();
        }

        public static void Register(ApplicationSettingsBase options)
        {
            optionList.Add(options);
        }
    }

    public sealed partial class Settings1
    {


        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public global::System.Collections.Hashtable ProfileConfigWizardStatus
        {
            get
            {
                return ((global::System.Collections.Hashtable)(this["ProfileConfigWizardStatus"]));
            }
            set
            {
                this["ProfileConfigWizardStatus"] = value;
            }
        }
    }
}
