using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Service.V2.Model;
using Microsoft.Windows.Controls.Ribbon;

namespace BodyArchitect.Client.Common.Plugins
{
    public interface IEntryObjectControl
    {
        void Fill(EntryObjectDTO entry, SaveTrainingDayResult originalEntry);
        void UpdateEntryObject();
        void AfterSave(bool isWindowClosing);
        bool ReadOnly { get; set; }
    }

    public interface IRibbonCreator
    {
        void CreateRibbonGroup(RibbonTab ribbonTab);
    }
}
