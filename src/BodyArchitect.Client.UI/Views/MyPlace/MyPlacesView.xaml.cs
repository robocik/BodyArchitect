using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Client.WCF;
using BodyArchitect.Logger;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Shared;
using Color = System.Drawing.Color;

namespace BodyArchitect.Client.UI.Views.MyPlace
{
    /// <summary>
    /// Interaction logic for MyPlacesView.xaml
    /// </summary>
    public partial class MyPlacesView : IWeakEventListener
    {
        private bool canEdit;
        private bool canDelete;
        private bool canNew;
        private bool canSetDefault;
        private MyPlaceDTO _selectedItem;
        ObservableCollection<MyPlaceDTO> items = new ObservableCollection<MyPlaceDTO>();
        private MyPlacesReposidory _myPlacesCache;

        public MyPlacesView()
        {
            InitializeComponent();
            DataContext = this;
            Header = EnumLocalizer.Default.GetGUIString("MyPlacesView_Header");
        }

        #region Properties


        public bool CanNew
        {
            get { return canNew; }
            set
            {
                canNew = value;
                NotifyOfPropertyChange(() => CanNew);
            }
        }

        public bool CanSetDefault
        {
            get { return canSetDefault; }
            set
            {
                canSetDefault = value;
                NotifyOfPropertyChange(() => CanSetDefault);
            }
        }
        
        public bool CanEdit
        {
            get { return canEdit; }
            set
            {
                canEdit = value;
                NotifyOfPropertyChange(() => CanEdit);
            }
        }

        public bool CanDelete
        {
            get { return canDelete; }
            set
            {
                canDelete = value;
                NotifyOfPropertyChange(() => CanDelete);
            }
        }


        public ICollection<MyPlaceDTO> Items
        {
            get { return items; }
        }

        void updateButtons()
        {
            CanNew = true;
            CanEdit = SelectedItem != null;
            CanDelete = SelectedItem != null && !SelectedItem.IsSystem;
            CanSetDefault = SelectedItem != null && !SelectedItem.IsDefault;
        }

        public MyPlaceDTO SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                updateButtons();
                NotifyOfPropertyChange(() => SelectedItem);
            }
        }
        #endregion

        

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            UIHelper.BeginInvoke(FillItems, Dispatcher);

            return true;
        }

        public void FillItems()
        {
            IsInProgress = true;
            ParentWindow.RunAsynchronousOperation(delegate(OperationContext context)
            {
                _myPlacesCache.EnsureLoaded();
                UIHelper.BeginInvoke(()=>
                    {
                        items.Clear();
                        foreach (var gym in _myPlacesCache.Items.Values)
                        {
                            items.Add(gym);
                        }
                        updateButtons();
                        IsInProgress = false;
                    },Dispatcher);

            });
        }

        public override void Fill()
        {
            _myPlacesCache = MyPlacesReposidory.GetCache(PageContext!=null?PageContext.User.GlobalId:(Guid?) null);
            CollectionChangedEventManager.AddListener(_myPlacesCache, this);
            FillItems();
        }

        public override void RefreshView()
        {
            _myPlacesCache.ClearCache();
            FillItems();
        }

        public override Uri HeaderIcon
        {
            get { return "MyGyms16.png".ToResourceUrl(); }
        }

        public override AccountType AccountType
        {
            get { return Service.V2.Model.AccountType.PremiumUser; }
        }

        private void lsItems_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                deleteSelectedItem();
            }
        }

        private void deleteSelectedItem()
        {
            if (SelectedItem != null && !SelectedItem.IsSystem && UIHelper.EnsurePremiumLicence() && BAMessageBox.AskYesNo(EnumLocalizer.Default.GetGUIString("MyPlacesView_QDeleteMyPlace"), SelectedItem.Name) == MessageBoxResult.Yes)
            {
                PleaseWait.Run(delegate(MethodParameters param)
                {
                    try
                    {
                        var param1 = new MyPlaceOperationParam();
                        param1.Operation = MyPlaceOperationType.Delete;
                        param1.MyPlaceId = SelectedItem.GlobalId;
                        ServiceManager.MyPlaceOperation(param1);
                        _myPlacesCache.Remove(SelectedItem.GlobalId);
                        UIHelper.BeginInvoke(() => NotifyOfPropertyChange(() => Items),Dispatcher);
                    }
                    catch (DeleteConstraintException ex)
                    {
                         
                        param.CloseProgressWindow();
                        UIHelper.BeginInvoke(() => ExceptionHandler.Default.Process(ex, EnumLocalizer.Default.GetGUIString("MyPlacesView_ErrDeleteMyPlace_IsInUse"), ErrorWindow.MessageBox), null);
                    }
                    catch (Exception ex)
                    {
                        param.CloseProgressWindow();
                        UIHelper.BeginInvoke(() => ExceptionHandler.Default.Process(ex, EnumLocalizer.Default.GetGUIString("MyPlacesView_ErrDeleteMyPlace"), ErrorWindow.EMailReport), null);

                    }
                });
            }
        }

        private void rbtnNew_Click(object sender, RoutedEventArgs e)
        {
            newItem();
        }

        private void newItem()
        {
            if(!UIHelper.EnsurePremiumLicence())
            {
                return;
            }
            var dlg = new EditDomainObjectWindow();

            var ctrl = new usrMyPlaceDetails();

            dlg.SetControl(ctrl);
            var myPlace = new MyPlaceDTO();
            myPlace.Color = Color.LavenderBlush.ToColorString();
            ctrl.Fill(myPlace);
            if (dlg.ShowDialog() == true)
            {
                _myPlacesCache.Add(ctrl.MyPlace);
                NotifyOfPropertyChange(() => Items);
            }
        }

        private void rbtnEdit_Click(object sender, RoutedEventArgs e)
        {
            editSelectedItem();
        }

        private void rbtnDelete_Click(object sender, RoutedEventArgs e)
        {
            deleteSelectedItem();
        }

        private void lsItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = (MyPlaceDTO)lsItems.GetClickedItem(e);
            if (item != null)
            {
                SelectedItem = item;
                editSelectedItem();
            }
        }

        private void editSelectedItem()
        {
            if (!UIHelper.EnsurePremiumLicence())
            {
                return;
            }
            var dlg = new EditDomainObjectWindow();

            var ctrl = new usrMyPlaceDetails();

            dlg.SetControl(ctrl);
            ctrl.Fill(SelectedItem.Clone());
            if (dlg.ShowDialog() == true)
            {
                replace(SelectedItem, ctrl.MyPlace);
            }
        }

        private void replace(MyPlaceDTO selected, MyPlaceDTO saved)
        {
            _myPlacesCache.Replace(selected, saved);
            SelectedItem = saved;
            NotifyOfPropertyChange(() => Items);
        }

        private void rbtnSetDefault_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem != null && UIHelper.EnsurePremiumLicence())
            {
                PleaseWait.Run(delegate(MethodParameters param)
                {
                    try
                    {
                        var param1 = new MyPlaceOperationParam();
                        param1.Operation = MyPlaceOperationType.SetDefault;
                        param1.MyPlaceId = SelectedItem.GlobalId;
                        var result = ServiceManager.MyPlaceOperation(param1);
                        var oldDefault = _myPlacesCache.Items.Values.Where(x => x.IsDefault).Single();
                        oldDefault.IsDefault = false;
                        replace(SelectedItem, result);
                    }
                    catch (Exception ex)
                    {
                        UIHelper.BeginInvoke(() => ExceptionHandler.Default.Process(ex, EnumLocalizer.Default.GetGUIString("MyPlacesView_ErrSetDefaultMyPlace"), ErrorWindow.EMailReport), null);
                    }
                });
            }
        }
    }
}
