using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using AvalonDock.Layout;
using BodyArchitect.Client.Common.Plugins;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.UI.UserControls
{
    public abstract class usrEntryObjectBase : usrBaseControl, IEntryObjectControl, IHasFloatingPane
    {
        protected usrEntryObjectDetailsBase detailsControl;
        protected usrEntryObjectUserControl progressControl;
        private EntryObjectDTO entry;
        private bool isFilling;

        protected usrEntryObjectBase()
        {
            if ( HasProgressPane)
            {
                MainWindow.Instance.EnsureAnchorable(Strings.usrEntryObjectBase_ProgressPanel_Header, "pack://application:,,,/BodyArchitect.Client.Resources;component/Images/Progress16.png", "EntryObjectProgress", AnchorableShowStrategy.Left);
            }
            if (HasDetailsPane )
            {
                MainWindow.Instance.EnsureAnchorable(Strings.usrEntryObjectBase_DetailsPanel_Header, "pack://application:,,,/BodyArchitect.Client.Resources;component/Images/Details16.png", "EntryObjectDetails", AnchorableShowStrategy.Left);
            }
            if (HasDetailsPane || HasProgressPane)
            {
                Loaded += new System.Windows.RoutedEventHandler(usrEntryObjectBase_Loaded);
            }
        }

        void usrEntryObjectBase_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.UpdateAnchorables();
        }

        public Control GetContentForPane(string paneId)
        {
            if (paneId == "EntryObjectProgress")
            {
                if (progressControl == null && HasProgressPane)
                {
                    progressControl = CreateProgressControl();
                    progressControl.Fill(entry);
                }
                
                return progressControl;
            }
            if (paneId == "EntryObjectDetails")
            {
                if (detailsControl == null && HasDetailsPane)
                {
                    detailsControl = CreateDetailsControl();
                    detailsControl.UpdateReadOnly(ReadOnly);
                    detailsControl.Fill(entry);
                    detailsControl.ObjectChanged += new EventHandler(detailsControl_ObjectChanged);
                }
                
                return detailsControl;
            }
            return null;
        }

        public EntryObjectDTO Entry
        {
            get { return entry; }
        }

        public virtual bool HasProgressPane
        {
            get { return false; }
        }

        public virtual bool HasDetailsPane
        {
            get { return false; }
        }

        public bool ReadOnly
        {
            get;
            set;
        }

        protected virtual usrEntryObjectUserControl CreateProgressControl()
        {
            return null;
        }

        protected virtual usrEntryObjectDetailsBase CreateDetailsControl()
        {
            return null;
        }

        void detailsControl_ObjectChanged(object sender, EventArgs e)
        {
            SetModifiedFlag();
        }

        protected abstract void FillImplementation(SaveTrainingDayResult originalEntry);

        public void Fill(EntryObjectDTO entry, SaveTrainingDayResult originalEntry)
        {
            isFilling = true;
            this.entry = entry;
            FillImplementation(originalEntry);
            if (detailsControl != null)
            {
                detailsControl.Fill(this.entry);
            }
            if (progressControl != null)
            {
                progressControl.Fill(this.entry);
            }
            isFilling = false;
        }

        public virtual void AfterSave(bool isWindowClosing)
        {

        }

        protected virtual void UpdateEntryObjectImplementation()
        {
            
        }
        public void UpdateEntryObject()
        {
            UpdateEntryObjectImplementation();
            if (detailsControl != null)
            {
                detailsControl.UpdateEntryObject(Entry);
            }
        }

        public void SetModifiedFlag()
        {
            if (Parent != null && !isFilling)
            {
                UIHelper.Invoke(() =>
                                    {
                                        UpdateEntryObject();
                                        UIHelper.FindVisualParent<TrainingDayWindow>(Parent).SetModifiedFlag();
                                    },Dispatcher);
                
            }
        }

        
    }
}
