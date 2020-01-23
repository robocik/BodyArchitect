using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Common.Plugins;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Settings.Model;
using Microsoft.Windows.Controls.Ribbon;

namespace BodyArchitect.Client.Module.Measurements
{
    [Export(typeof(IEntryObjectProvider))]
    [Export(typeof(IBodyArchitectModule))]
    public class SizeModule : IEntryObjectProvider, IBodyArchitectModule
    {
        public static readonly Guid ModuleId = new Guid("083F5171-0D6C-46B4-B72A-155DB3C0270A");

        public Guid GlobalId
        {
            get { return ModuleId; }
        }


        public void Startup(EntryObjectLocalizationManager localizationManager, EntryObjectControlManager controlManager)
        {
            localizationManager.RegisterResourceManager(typeof(SizeEntryDTO), SizeEntryStrings.ResourceManager);
            //controlManager.RegisterControl<SizeEntryDTO>(typeof(usrMeasurementsEntry));
        }

        public void AfterUserLogin()
        {
            
        }

        public Type EntryObjectControl
        {
            get { return typeof(usrMeasurementsEntry); }
        }

        public ImageSource ModuleImage
        {
            get
            {
                return "pack://application:,,,/BodyArchitect.Client.Module.Measurements;component/Resources/Measurements.png".ToBitmap();
            }
        }

        public Type EntryObjectType
        {
            get { return typeof(SizeEntryDTO); }
        }

        public ShareSocialContent ShareToSocial(EntryObjectDTO entryObj)
        {

            var sizeEntry = (SizeEntryDTO)entryObj;
            ShareSocialContent content = new ShareSocialContent();
            content.Caption = EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.Measurements:SizeEntryStrings:SizeModule_ShareToSocial_Caption");
            content.Name = EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.Measurements:SizeEntryStrings:SizeModule_ShareToSocial_Name");
            StringBuilder descriptionBuilder = new StringBuilder();
            descriptionBuilder.AppendFormat(EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.Measurements:SizeEntryStrings:SizeModule_ShareToSocial_Date"), sizeEntry.TrainingDay.TrainingDate.ToShortDateString());
            string weightType="WeightType_Kg".TranslateStrings();
            string lengthType = "LengthType_Cm".TranslateStrings();
            if (UserContext.Current.ProfileInformation.Settings.WeightType == WeightType.Pounds)
            {
                weightType = "WeightType_Pound".TranslateStrings();
            }
            if (UserContext.Current.ProfileInformation.Settings.LengthType == LengthType.Inchs)
            {
                lengthType = "LengthType_Inch".TranslateStrings();
            }

            addMeasurement(descriptionBuilder, sizeEntry.Wymiary.Klatka.ToDisplayLength(),"usrWymiaryEditor_KlatkaLabel".TranslateGUI(), lengthType);
            addMeasurement(descriptionBuilder, sizeEntry.Wymiary.RightBiceps.ToDisplayLength(), "usrWymiaryEditor_RightBicepsLabel".TranslateGUI(), lengthType);
            addMeasurement(descriptionBuilder, sizeEntry.Wymiary.LeftBiceps.ToDisplayLength(), "usrWymiaryEditor_LeftBicepsLabel".TranslateGUI(), lengthType);
            addMeasurement(descriptionBuilder, sizeEntry.Wymiary.RightForearm.ToDisplayLength(), "usrWymiaryEditor_RightForearmLabel".TranslateGUI(), lengthType);
            addMeasurement(descriptionBuilder, sizeEntry.Wymiary.LeftForearm.ToDisplayLength(), "usrWymiaryEditor_LeftForearmsLabel".TranslateGUI(), lengthType);
            addMeasurement(descriptionBuilder, sizeEntry.Wymiary.RightUdo.ToDisplayLength(), "usrWymiaryEditor_RightUdoLabel".TranslateGUI(), lengthType);
            addMeasurement(descriptionBuilder, sizeEntry.Wymiary.LeftUdo.ToDisplayLength(), "usrWymiaryEditor_LeftUdoLabel".TranslateGUI(), lengthType);
            addMeasurement(descriptionBuilder, sizeEntry.Wymiary.Pas.ToDisplayLength(), "usrWymiaryEditor_PasLabel".TranslateGUI(), lengthType);
            addMeasurement(descriptionBuilder, sizeEntry.Wymiary.Weight.ToDisplayWeight(), "usrWymiaryEditor_WeightLabel".TranslateGUI(), weightType);

            content.Description = descriptionBuilder.ToString();

            return content;
        }

        void addMeasurement(StringBuilder builder, decimal value, string label, string unit)
        {
            if (value > 0)
            {
                builder.AppendFormat(EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.Measurements:SizeEntryStrings:SizeModule_addMeasurement"), label.Replace("_", ""), value, unit);
            }
        }

        
        public void CreateRibbon(Ribbon ribbon)
        {

        }

    }
}
