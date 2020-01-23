using BodyArchitect.Service.Model;

namespace BodyArchitect.Common.Controls
{
    public interface IEntryObjectControl
    {
        void Fill(EntryObjectDTO entry);
        void UpdateEntryObject();
        void AfterSave(bool isWindowClosing);
        bool ReadOnly { get; set; }
    }
}
