using System;
using BodyArchitect.Common.Controls;
using BodyArchitect.Controls;
using BodyArchitect.Module.Suplements.Model;
using BodyArchitect.Service.Model;

namespace BodyArchitect.Module.Suplements.Controls
{
    public partial class usrSuplements : usrBaseControl, IEntryObjectControl, IValidationControl
    {
        private SuplementsEntryDTO suplementEntry;

        public usrSuplements()
        {
            InitializeComponent();
        }

        public void Fill(EntryObjectDTO entry)
        {
            suplementEntry = (SuplementsEntryDTO)entry;
            updateReadOnly();
            suplementsGrid1.Fill(suplementEntry);
            usrReportStatus1.Fill(entry);
        }

        void updateReadOnly()
        {
            usrReportStatus1.ReadOnly = ReadOnly;
            suplementsGrid1.ReadOnly = ReadOnly;
            usrReportStatus1.Visible = !ReadOnly;
        }
        public void UpdateEntryObject()
        {
            var items = suplementsGrid1.GetSuplementItems();
            suplementEntry.Items.Clear();
            usrReportStatus1.Save(suplementEntry);
            foreach (var dayEntry in items)
            {
                suplementEntry.AddItem(dayEntry);
            }
        }

        public void AfterSave(bool isWindowClosing)
        {

        }

        public bool ReadOnly
        {
            get; set; 
        }

        public bool ValidateControl()
        {
            return true;
        }
    }
}
