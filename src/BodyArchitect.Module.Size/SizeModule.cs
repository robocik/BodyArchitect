using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using BodyArchitect.WCF;
using BodyArchitect.Common.Plugins;
using BodyArchitect.Common.Localization;
using System.Reflection;
using BodyArchitect.Controls;
using BodyArchitect.Service.Model;
using BodyArchitect.Settings;
using BodyArchitect.Module.Size.Controls;
using System.Drawing;
using BodyArchitect.Common;
using BodyArchitect.Module.Size.Reporting;
using BodyArchitect.Shared;

namespace BodyArchitect.Module.Size
{
    [Export(typeof(IEntryObjectProvider))]
    public class SizeModule : IEntryObjectProvider
    {
        public static readonly Guid ModuleId = new Guid("083F5171-0D6C-46B4-B72A-155DB3C0270A");
        public Guid GlobalId
        {
            get { return ModuleId; }
        }


        public void Startup(EntryObjectLocalizationManager localizationManager, EntryObjectControlManager controlManager)
        {
            localizationManager.RegisterResourceManager(typeof(SizeEntryDTO), SizeEntryStrings.ResourceManager);
            controlManager.RegisterControl<SizeEntryDTO>(typeof(usrSizeEntry));
        }


        public Image ModuleImage
        {
            get { return SizeResources.SizeModule; }
        }

        public Type EntryObjectType
        {
            get { return typeof(SizeEntryDTO); }
        }

        public void CreateGui(IMainWindow mainWnd)
        {
            
        }

        public void PrepareNewEntryObject(SessionData sessionData, EntryObjectDTO obj, TrainingDayDTO day)
        {
            
        }

        public void CreateMainMenuItems(MenuStrip menu)
        {
          
        }

        public void AfterSave(SessionData sessionData, TrainingDayDTO day)
        {
            //if (BodyArchitect.Module.Size.Options.SizeModuleSettings.Default.UpdateProfileSizes)
            //{
            //    var sizeEntries = day.GetSpecifiedEntries<SizeEntryDTO>();
            //    if (sizeEntries.Count == 1)
            //    {
            //        if (UserContext.ProfileInformation.Wymiary == null)
            //        {
            //            UserContext.ProfileInformation.Wymiary = new WymiaryDTO();
            //        }
            //        else if (UserContext.ProfileInformation.Wymiary.DateTime > sizeEntries[0].Wymiary.DateTime)
            //        {//if we fill some old size entry then we shouldn't update profile. Profile is only updated with the new (current) entries
            //            return;
            //        }
            //        if (sizeEntries[0].Wymiary.Height > 0)
            //        {
            //            UserContext.ProfileInformation.Wymiary.Height = sizeEntries[0].Wymiary.Height;
            //        }

            //        UserContext.ProfileInformation.Wymiary.Klatka = getLatestValue(UserContext.ProfileInformation.Wymiary.Klatka, sizeEntries[0].Wymiary.Klatka);
            //        UserContext.ProfileInformation.Wymiary.LeftBiceps = getLatestValue(UserContext.ProfileInformation.Wymiary.LeftBiceps, sizeEntries[0].Wymiary.LeftBiceps);
            //        UserContext.ProfileInformation.Wymiary.LeftForearm = getLatestValue(UserContext.ProfileInformation.Wymiary.LeftForearm, sizeEntries[0].Wymiary.LeftForearm);
            //        UserContext.ProfileInformation.Wymiary.LeftUdo = getLatestValue(UserContext.ProfileInformation.Wymiary.LeftUdo, sizeEntries[0].Wymiary.LeftUdo);
            //        UserContext.ProfileInformation.Wymiary.Pas = getLatestValue(UserContext.ProfileInformation.Wymiary.Pas, sizeEntries[0].Wymiary.Pas);
            //        UserContext.ProfileInformation.Wymiary.RightBiceps = getLatestValue(UserContext.ProfileInformation.Wymiary.RightBiceps, sizeEntries[0].Wymiary.RightBiceps);
            //        UserContext.ProfileInformation.Wymiary.RightForearm = getLatestValue(UserContext.ProfileInformation.Wymiary.RightForearm, sizeEntries[0].Wymiary.RightForearm);
            //        UserContext.ProfileInformation.Wymiary.RightUdo = getLatestValue(UserContext.ProfileInformation.Wymiary.RightUdo, sizeEntries[0].Wymiary.RightUdo);
            //        UserContext.ProfileInformation.Wymiary.Weight = getLatestValue(UserContext.ProfileInformation.Wymiary.Weight, sizeEntries[0].Wymiary.Weight);
            //        UserContext.ProfileInformation.Wymiary.DateTime = DateHelper.MoveToNewDate(sizeEntries[0].Wymiary.DateTime,sizeEntries[0].TrainingDay.TrainingDate);
            //        ProfileUpdateData data = new ProfileUpdateData();
            //        data.Profile = UserContext.CurrentProfile;
            //        data.Wymiary = UserContext.ProfileInformation.Wymiary;
            //        ServiceManager.UpdateProfile(data);
            //        UserContext.RefreshUserData();
            //    }
            //}

            
        }

        float getLatestValue(float oldValue,float newValue)
        {
            if(newValue>0)
            {
                return newValue;
            }
            return oldValue;
        }
    }
}
