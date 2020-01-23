using System;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Module.Measurements
{
    /// <summary>
    /// Interaction logic for usrMeasurementsEntry.xaml
    /// </summary>
    public partial class usrMeasurementsEntry
    {
        public SizeEntryDTO SizeEntry
        {
            get { return (SizeEntryDTO) Entry; }
        }
        public usrMeasurementsEntry()
        {
            InitializeComponent();
        }

        public override bool HasDetailsPane
        {
            get { return true; }
        }

        public override bool HasProgressPane
        {
            get { return true; }
        }

        protected override UI.UserControls.usrEntryObjectUserControl CreateProgressControl()
        {
            return new usrMeasurementsProgress();
        }

        protected override UI.UserControls.usrEntryObjectDetailsBase CreateDetailsControl()
        {
            var ctrl= new usrMeasurementsEntryDetails();
            ctrl.UpdateReadOnly(ReadOnly);
            return ctrl;
        }


        protected override void FillImplementation(SaveTrainingDayResult originalEntry)
        {
            updateReadOnly();
            this.usrWymiaryEditor1.Fill(SizeEntry.Wymiary);
        }

        protected override void UpdateEntryObjectImplementation()
        {
            usrWymiaryEditor1.SaveWymiary(SizeEntry.Wymiary);
        }

        void updateReadOnly()
        {
            usrWymiaryEditor1.ReadOnly = ReadOnly;
            if(detailsControl!=null)
            {
                detailsControl.UpdateReadOnly(ReadOnly);
            }
        }

        public Type EntryObjectType
        {
            get { return typeof(SizeEntryDTO); }
        }

        private void usrWymiaryEditor1_MeasurementChanged(object sender, EventArgs e)
        {
            SetModifiedFlag();
        }

        
    }
}
