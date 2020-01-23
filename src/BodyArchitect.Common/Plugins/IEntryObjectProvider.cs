using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.Common.Localization;
using System.Drawing;
using BodyArchitect.Service.Model;

namespace BodyArchitect.Common.Plugins
{
    public interface IEntryObjectProvider
    {
        Guid GlobalId { get; }
        void Startup( EntryObjectLocalizationManager localizationManager, EntryObjectControlManager controlManager);

        Image ModuleImage
        {
            get;
        }

        Type EntryObjectType
        {
            get;
        }

        void CreateGui(IMainWindow mainWnd);

        void PrepareNewEntryObject(SessionData sessionData, EntryObjectDTO obj, TrainingDayDTO day);

        void CreateMainMenuItems(MenuStrip menu);

        void AfterSave(SessionData sessionData,TrainingDayDTO day);
    }
}
