using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.IsolatedStorage;
using BodyArchitect.Service.V2.Model;
using Coding4Fun.Phone.Controls.Data;


namespace BodyArchitect.Service.Client.WP7
{
    public enum ExerciseSortBy
    {
        Name,
        Shortcut
    }

    public enum CopyStrengthTrainingMode
    {
        Full,
        WithoutSetsData,
        OnlyExercises
    }

    public class Settings
    {
        public const string AutoPauseKey = "AutoPause";
        public const string LocationServicesKey = "LocationServices";
        public const string RunUnderLockScreenKey = "RunUnderLockScreen";
        public const string ExercisesLanguageKey ="ExercisesLanguage";
        public const string ExercisesSortByKey = "ExercisesSortBy";
        public const string NewSetCopyValuesKey = "NewSetCopyValues";
        public const string ClientInstanceIdKey = "ClientInstanceId";
        public const string NumberOfMonthToRetrieveKey = "NumberOfMonthToRetrieve";
        public const string TipsDateTimeKey = "TipsDateTime";
        public const string LiveTileEnabledKey = "LiveTileEnabled";
        public const string SendUsageDataKey = "SendUsageData";
        public const string SendCrashDataKey = "SendCrashData";
        public const string InitialAskKey = "InitalAsk";
        public const string UserNameKey = "UserNameKey";
        public const string PasswordKey = "PasswordKey";
        public const string RefreshFrequencyKey = "RefreshFrequencyKey";
        public const string CopyStrengthTrainingModeKey = "CopyStrengthTrainingMode";
        public const string RunsCountKey = "RunsCount";
        public const string TimerExpandedKey = "TimerExpanded";
        //public const string WeightTypeKey = "WeightType";
        //public const string LengthTypeKey = "LengthType";
        public const string ShowSystemTrayKey = "ShowSystemTray";
        public const string TreatSuperSetsAsOneKey = "TreatSuperSetsAsOne";
        public const string StartTimerKey = "StartTimer";

        public const string InfoOfflineModeKey = "InfoOfflineMode";

        public static ClientInformation GetClientInformation()
        {
            ClientInformation info = new ClientInformation();

            info.ApplicationLanguage = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            info.ApplicationVersion = PhoneHelper.GetAppAttribute("Version");
            info.Version = Constants.ServiceVersion;
            info.ClientInstanceId = ClientInstanceId;
            info.Platform = PlatformType.WindowsPhone;
            info.PlatformVersion = Environment.OSVersion.ToString();
            return info;
        }

        public static IDictionary<string, string> GetDictionary()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            //dict.Add("consent",SendCrashData.ToString());
            return dict;
        }

        public static bool AutoPause
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(AutoPauseKey))
                {
                    return (bool)IsolatedStorageSettings.ApplicationSettings[AutoPauseKey];
                }
                return false;//autopause is disable in default settings
            }
            set
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(AutoPauseKey))
                {
                    IsolatedStorageSettings.ApplicationSettings[AutoPauseKey] = value;
                }
                else
                {
                    IsolatedStorageSettings.ApplicationSettings.Add(AutoPauseKey, value);
                }
            }
        }

        public static bool RunUnderLockScreen
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(RunUnderLockScreenKey))
                {
                    return (bool)IsolatedStorageSettings.ApplicationSettings[RunUnderLockScreenKey];
                }
                return true;
            }
            set
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(RunUnderLockScreenKey))
                {
                    IsolatedStorageSettings.ApplicationSettings[RunUnderLockScreenKey] = value;
                }
                else
                {
                    IsolatedStorageSettings.ApplicationSettings.Add(RunUnderLockScreenKey, value);
                }
            }
        }

        public static bool LocationServices
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(LocationServicesKey))
                {
                    return (bool)IsolatedStorageSettings.ApplicationSettings[LocationServicesKey];
                }
                return true;
            }
            set
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(LocationServicesKey))
                {
                    IsolatedStorageSettings.ApplicationSettings[LocationServicesKey] = value;
                }
                else
                {
                    IsolatedStorageSettings.ApplicationSettings.Add(LocationServicesKey, value);
                }
            }
        }

        public static bool TreatSuperSetsAsOne
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(TreatSuperSetsAsOneKey))
                {
                    return (bool)IsolatedStorageSettings.ApplicationSettings[TreatSuperSetsAsOneKey];
                }
                return false;
            }
            set
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(TreatSuperSetsAsOneKey))
                {
                    IsolatedStorageSettings.ApplicationSettings[TreatSuperSetsAsOneKey] = value;
                }
                else
                {
                    IsolatedStorageSettings.ApplicationSettings.Add(TreatSuperSetsAsOneKey, value);
                }
            }
        }

        public static bool TimerExpanded
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(TimerExpandedKey))
                {
                    return (bool)IsolatedStorageSettings.ApplicationSettings[TimerExpandedKey];
                }
                return false;
            }
            set
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(TimerExpandedKey))
                {
                    IsolatedStorageSettings.ApplicationSettings[TimerExpandedKey] = value;
                }
                else
                {
                    IsolatedStorageSettings.ApplicationSettings.Add(TimerExpandedKey, value);
                }
            }
        }

        public static string ServerUrl
        {
            get
            {
#if DEBUG
                return "http://test.bodyarchitectonline.com/";
#else
                return "http://service.bodyarchitectonline.com/";
#endif
            }
        }
        public static string ClientId
        {
            get { return ClientInstanceId.ToString(); }
        }

        public static bool InfoOfflineMode
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(InfoOfflineModeKey))
                {
                    return (bool)IsolatedStorageSettings.ApplicationSettings[InfoOfflineModeKey];
                }
                return false;
            }
            set
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(InfoOfflineModeKey))
                {
                    IsolatedStorageSettings.ApplicationSettings[InfoOfflineModeKey] = value;
                }
                else
                {
                    IsolatedStorageSettings.ApplicationSettings.Add(InfoOfflineModeKey, value);
                }
            }
        }

        public static bool ShowSystemTray
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(ShowSystemTrayKey))
                {
                    return (bool)IsolatedStorageSettings.ApplicationSettings[ShowSystemTrayKey];
                }
                return true;
            }
            set
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(ShowSystemTrayKey))
                {
                    IsolatedStorageSettings.ApplicationSettings[ShowSystemTrayKey] = value;
                }
                else
                {
                    IsolatedStorageSettings.ApplicationSettings.Add(ShowSystemTrayKey, value);
                }
            }
        }

        //public static WeightType WeightType
        //{
        //    get
        //    {
        //        if (IsolatedStorageSettings.ApplicationSettings.Contains(WeightTypeKey))
        //        {
        //            return (WeightType)IsolatedStorageSettings.ApplicationSettings[WeightTypeKey];
        //        }
        //        return WeightType.Kg;
        //    }
        //    set
        //    {
        //        if (IsolatedStorageSettings.ApplicationSettings.Contains(WeightTypeKey))
        //        {
        //            IsolatedStorageSettings.ApplicationSettings[WeightTypeKey] = value;
        //        }
        //        else
        //        {
        //            IsolatedStorageSettings.ApplicationSettings.Add(WeightTypeKey, value);
        //        }
        //    }
        //}

        //public static LengthType LengthType
        //{
        //    get
        //    {
        //        if (IsolatedStorageSettings.ApplicationSettings.Contains(LengthTypeKey))
        //        {
        //            return (LengthType)IsolatedStorageSettings.ApplicationSettings[LengthTypeKey];
        //        }
        //        return LengthType.Cm;
        //    }
        //    set
        //    {
        //        if (IsolatedStorageSettings.ApplicationSettings.Contains(LengthTypeKey))
        //        {
        //            IsolatedStorageSettings.ApplicationSettings[LengthTypeKey] = value;
        //        }
        //        else
        //        {
        //            IsolatedStorageSettings.ApplicationSettings.Add(LengthTypeKey, value);
        //        }
        //    }
        //}

        public static long RunsCount
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(RunsCountKey))
                {
                    return (long)IsolatedStorageSettings.ApplicationSettings[RunsCountKey];
                }
                return 0;
            }
            set
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(RunsCountKey))
                {
                    IsolatedStorageSettings.ApplicationSettings[RunsCountKey] = value;
                }
                else
                {
                    IsolatedStorageSettings.ApplicationSettings.Add(RunsCountKey, value);
                }
            }
        }


        public static CopyStrengthTrainingMode CopyStrengthTrainingMode
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(CopyStrengthTrainingModeKey))
                {
                    return (CopyStrengthTrainingMode)IsolatedStorageSettings.ApplicationSettings[CopyStrengthTrainingModeKey];
                }
                return CopyStrengthTrainingMode.WithoutSetsData;
            }
            set
            {

                if (IsolatedStorageSettings.ApplicationSettings.Contains(CopyStrengthTrainingModeKey))
                {
                    IsolatedStorageSettings.ApplicationSettings[CopyStrengthTrainingModeKey] = value;
                }
                else
                {
                    IsolatedStorageSettings.ApplicationSettings.Add(CopyStrengthTrainingModeKey, value);
                }
            }
        }

        public static Guid ClientInstanceId
        {
            get
            {
                if (!IsolatedStorageSettings.ApplicationSettings.Contains(ClientInstanceIdKey))
                {
                    IsolatedStorageSettings.ApplicationSettings[ClientInstanceIdKey]=Guid.NewGuid();
                }
                return (Guid)IsolatedStorageSettings.ApplicationSettings[ClientInstanceIdKey]; 
            }
        }

        //public static int RefreshFrequencyDays
        //{
        //    get
        //    {
        //        if (IsolatedStorageSettings.ApplicationSettings.Contains(RefreshFrequencyKey))
        //        {
        //            return (int)IsolatedStorageSettings.ApplicationSettings[RefreshFrequencyKey];
        //        }
        //        return 7;
        //    }
        //    set
        //    {

        //        if (IsolatedStorageSettings.ApplicationSettings.Contains(RefreshFrequencyKey))
        //        {
        //            IsolatedStorageSettings.ApplicationSettings[RefreshFrequencyKey] = value;
        //        }
        //        else
        //        {
        //            IsolatedStorageSettings.ApplicationSettings.Add(RefreshFrequencyKey, value);
        //        }
        //    }
        //}

        public static bool InitialAsk
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(InitialAskKey))
                {
                    return (bool)IsolatedStorageSettings.ApplicationSettings[InitialAskKey];
                }
                return false;
            }
            set
            {

                if (IsolatedStorageSettings.ApplicationSettings.Contains(InitialAskKey))
                {
                    IsolatedStorageSettings.ApplicationSettings[InitialAskKey] = value;
                }
                else
                {
                    IsolatedStorageSettings.ApplicationSettings.Add(InitialAskKey, value);
                }
            }
        }

        public static string UserName
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(UserNameKey))
                {
                    return (string)IsolatedStorageSettings.ApplicationSettings[UserNameKey];
                }
                return null;
            }
            set
            {

                if (IsolatedStorageSettings.ApplicationSettings.Contains(UserNameKey))
                {
                    IsolatedStorageSettings.ApplicationSettings[UserNameKey] = value;
                }
                else
                {
                    IsolatedStorageSettings.ApplicationSettings.Add(UserNameKey, value);
                }
            }
        }

        public static string Password
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(PasswordKey))
                {
                    return (string)IsolatedStorageSettings.ApplicationSettings[PasswordKey];
                }
                return null;
            }
            set
            {

                if (IsolatedStorageSettings.ApplicationSettings.Contains(PasswordKey))
                {
                    IsolatedStorageSettings.ApplicationSettings[PasswordKey] = value;
                }
                else
                {
                    IsolatedStorageSettings.ApplicationSettings.Add(PasswordKey, value);
                }
            }
        }

        public static DateTime? TipsDateTime
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(TipsDateTimeKey))
                {
                    return (DateTime)IsolatedStorageSettings.ApplicationSettings[TipsDateTimeKey];
                }
                return null;
            }
            set
            {

                if (IsolatedStorageSettings.ApplicationSettings.Contains(TipsDateTimeKey))
                {
                    IsolatedStorageSettings.ApplicationSettings[TipsDateTimeKey] = value;
                }
                else
                {
                    IsolatedStorageSettings.ApplicationSettings.Add(TipsDateTimeKey, value);
                }
            }
        }

        public static int NumberOfMonthToRetrieve
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(NumberOfMonthToRetrieveKey))
                {
                    return (int)IsolatedStorageSettings.ApplicationSettings[NumberOfMonthToRetrieveKey];
                }
                return 2;
            }
            set
            {
                if (value<1)
                {
                    value = 1;
                }
                if (IsolatedStorageSettings.ApplicationSettings.Contains(NumberOfMonthToRetrieveKey))
                {
                    IsolatedStorageSettings.ApplicationSettings[NumberOfMonthToRetrieveKey] = value;
                }
                else
                {
                    IsolatedStorageSettings.ApplicationSettings.Add(NumberOfMonthToRetrieveKey, value);
                }
            }
        }

        public static string ExercisesLanguage
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(ExercisesLanguageKey))
                {
                    var value= (string)IsolatedStorageSettings.ApplicationSettings[ExercisesLanguageKey];
                    //temporary for upgrade from old version (added in 2.3)
                    if(value=="pl")
                    {
                        ExercisesLanguage = "pl-PL";
                        return ExercisesLanguage;
                    }
                    else if(value=="en")
                    {
                        ExercisesLanguage = "en-US";
                        return ExercisesLanguage;
                    }
                    return value;
                }
                return null;
            }
            set
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(ExercisesLanguageKey))
                {
                    IsolatedStorageSettings.ApplicationSettings[ExercisesLanguageKey]=value;
                }
                else
                {
                    IsolatedStorageSettings.ApplicationSettings.Add(ExercisesLanguageKey,value);
                }
            }
        }

        public static ExerciseSortBy ExercisesSortBy
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(ExercisesSortByKey))
                {
                    return (ExerciseSortBy)IsolatedStorageSettings.ApplicationSettings[ExercisesSortByKey];
                }
                return ExerciseSortBy.Name;
            }
            set
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(ExercisesSortByKey))
                {
                    IsolatedStorageSettings.ApplicationSettings[ExercisesSortByKey] = value;
                }
                else
                {
                    IsolatedStorageSettings.ApplicationSettings.Add(ExercisesSortByKey, value);
                }
            }
        }

        public static bool CopyValuesForNewSet
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(NewSetCopyValuesKey))
                {
                    return (bool)IsolatedStorageSettings.ApplicationSettings[NewSetCopyValuesKey];
                }
                return true;
            }
            set
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(NewSetCopyValuesKey))
                {
                    IsolatedStorageSettings.ApplicationSettings[NewSetCopyValuesKey] = value;
                }
                else
                {
                    IsolatedStorageSettings.ApplicationSettings.Add(NewSetCopyValuesKey, value);
                }
            }
        }

        public static bool LiveTileEnabled
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(LiveTileEnabledKey))
                {
                    return (bool)IsolatedStorageSettings.ApplicationSettings[LiveTileEnabledKey];
                }
                return true;
            }
            set
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(LiveTileEnabledKey))
                {
                    IsolatedStorageSettings.ApplicationSettings[LiveTileEnabledKey] = value;
                }
                else
                {
                    IsolatedStorageSettings.ApplicationSettings.Add(LiveTileEnabledKey, value);
                }
            }
        }

        public static bool StartTimer
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(StartTimerKey))
                {
                    return (bool)IsolatedStorageSettings.ApplicationSettings[StartTimerKey];
                }
                return true;
            }
            set
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(StartTimerKey))
                {
                    IsolatedStorageSettings.ApplicationSettings[StartTimerKey] = value;
                }
                else
                {
                    IsolatedStorageSettings.ApplicationSettings.Add(StartTimerKey, value);
                }
            }
        }

        public static bool SendUsageData
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(SendUsageDataKey))
                {
                    return (bool)IsolatedStorageSettings.ApplicationSettings[SendUsageDataKey];
                }
                return true;
            }
            set
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(SendUsageDataKey))
                {
                    IsolatedStorageSettings.ApplicationSettings[SendUsageDataKey] = value;
                }
                else
                {
                    IsolatedStorageSettings.ApplicationSettings.Add(SendUsageDataKey, value);
                }
            }
        }

        public static bool SendCrashData
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(SendCrashDataKey))
                {
                    return (bool)IsolatedStorageSettings.ApplicationSettings[SendCrashDataKey];
                }
                return true;
            }
            set
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(SendCrashDataKey))
                {
                    IsolatedStorageSettings.ApplicationSettings[SendCrashDataKey] = value;
                }
                else
                {
                    IsolatedStorageSettings.ApplicationSettings.Add(SendCrashDataKey, value);
                }
            }
        }
    }
}
