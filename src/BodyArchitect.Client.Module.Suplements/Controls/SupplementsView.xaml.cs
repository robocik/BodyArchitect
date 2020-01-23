using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Logger;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;

namespace BodyArchitect.Client.Module.Suplements.Controls
{
    /// <summary>
    /// Interaction logic for SupplementsView.xaml
    /// </summary>
    public partial class SupplementsView 
    {
        public SupplementsView()
        {
            InitializeComponent();
            Header = EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.Suplements:SuplementsEntryStrings:SupplementsView_Header_Supplements_xamlCs");
            DataContext = this;
            
        }

        #region Ribbon

        private bool addEnable;
        private bool deleteEnable;
        private bool editEnable;

        public bool AddEnable
        {
            get { return addEnable; }
            set
            {
                addEnable = value;
                NotifyOfPropertyChange(()=>AddEnable);
            }
        }

        public bool DeleteEnable
        {
            get { return deleteEnable; }
            set
            {
                deleteEnable = value;
                NotifyOfPropertyChange(() => DeleteEnable);
            }
        }

        public bool EditEnable
        {
            get { return editEnable; }
            set
            {
                editEnable = value;
                NotifyOfPropertyChange(() => EditEnable);
            }
        }
        #endregion

        void fillSuperTips()
        {
            //TODO:Add tooltips
            //ControlHelper.AddSuperTip(this.defaultToolTipController1.DefaultController, this.lvSuplements, null, SuplementsEntryStrings.SuplementsViewWindow_SuplementsListView);
            //ControlHelper.AddSuperTip(this.btnAdd, btnAdd.Text, SuplementsEntryStrings.SuplementsViewWindow_AddBTN);
            //ControlHelper.AddSuperTip(this.btnEdit, btnEdit.Text, SuplementsEntryStrings.SuplementsViewWindow_EditBTN);
            //ControlHelper.AddSuperTip(this.btnDelete, btnDelete.Text, SuplementsEntryStrings.SuplementsViewWindow_DeleteBTN);
        }

        public override void Fill()
        {
            IsInProgress = true;
            lvSuplements.Items.Clear();
            if (UserContext.Current.LoginStatus != LoginStatus.Logged)
            {
                IsInProgress = false;
                return;
            }

            ParentWindow.RunAsynchronousOperation(delegate(OperationContext ctx)
            {
                SuplementsReposidory.Instance.EnsureLoaded();
                Dispatcher.BeginInvoke(new Action(delegate
                  {
                      foreach (SuplementDTO suplement in SuplementsReposidory.Instance.Items.Values)
                      {
                          ListItem<SuplementDTO> item = new ListItem<SuplementDTO>(suplement.Name, suplement);
                          lvSuplements.Items.Add(item);
                      }
                      IsInProgress = false;
                  }));

            }, asyncOperationStateChange);

            
        }

        void asyncOperationStateChange(OperationContext context)
        {
            bool startLoginOperation = context.TaskManager.StartedTasksCount > 1 || context.State == OperationState.Started;

            updateButtons(startLoginOperation);
        }

        public override void RefreshView()
        {
            SuplementsReposidory.Instance.ClearCache();
            Fill();
        }
        

        public override Uri HeaderIcon
        {
            get { return new Uri("pack://application:,,,/BodyArchitect.Client.Module.Suplements;component/Resources/Supplements.png", UriKind.Absolute); }
        }

        void viewSuplement(SuplementDTO suplement)
        {
            //TODO:FINISH
            //SuplementTypeEditor dlg = new SuplementTypeEditor();
            //dlg.Fill(suplement);
            //if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            //{
            //    Fill();
            //}
        }

        void updateButtons(bool operationStarted=false)
        {

            EditEnable = !operationStarted && lvSuplements.SelectedItems.Count == 1;
            DeleteEnable =!operationStarted && lvSuplements.SelectedItems.Count > 0;
            AddEnable = false;
            DeleteEnable = false;
            EditEnable = false;
        }


        private void deleteSuplements()
        {
            if (lvSuplements.SelectedItems.Count == 0)
            {
                return;
            }
            if (BAMessageBox.AskYesNo(EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.Suplements:SuplementsEntryStrings:QDeleteSuplementType")) == MessageBoxResult.No)
            {
                return;
            }
            try
            {
                //using (var scope = new TransactionScope())
                //{
                //    foreach (var selectedItem in lvSuplements.SelectedItems)
                //    {
                //        Suplement suplement = (Suplement)((ListViewItem)selectedItem).Tag;
                //        suplement.Delete();

                //    }
                //    scope.VoteCommit();
                //}
                MessageBox.Show(EnumLocalizer.Default.GetUIString("Must be implemented"));
                Fill();
            }
            catch (Exception ex)
            {
                ExceptionHandler.Default.Process(ex, EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.Suplements:SuplementsEntryStrings:ErrorCannotDeleteSuplementType"), ErrorWindow.EMailReport);
            }
        }


        private void lvSuplements_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                deleteSuplements();
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            viewSuplement(new SuplementDTO());
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            var suplement = (SuplementDTO)lvSuplements.SelectedItems[0];
            viewSuplement(suplement);
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            deleteSuplements();
        }

        private void lvSuplements_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            updateButtons();
        }

        private void lvSuplements_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = (ListItem<SuplementDTO>)lvSuplements.SelectedItem;

            //var item = lvSuplements.GetItemAt(e.X, e.Y);
            if (item != null)
            {
                viewSuplement(item.Value);
            }
        }
    }
}
