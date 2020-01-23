using System;
using System.Drawing;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.WCF.Plugins
{
    public interface IEntryObjectProvider
    {
        Guid GlobalId { get; }
        void Startup( /*EntryObjectLocalizationManager localizationManager*/ EntryObjectControlManager controlManager);

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

        void AfterSave(SessionData sessionData,TrainingDayDTO day);
    }
}
