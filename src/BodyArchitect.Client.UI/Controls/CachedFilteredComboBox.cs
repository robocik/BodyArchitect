using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.UI.Controls.WatermarkExtension;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;

namespace BodyArchitect.Client.UI.Controls
{
    public abstract class CachedFilteredComboBox<T> : FilteredComboBox, IWeakEventListener where T : BAGlobalObject
    {
        private ObjectsCacheBase<T> cache;


        public CachedFilteredComboBox()
        {
            this.Style = new Style(GetType(), this.FindResource("LoadingComboBox") as Style);
            
            //ItemsSource = new List<CustomerDTO>(CustomersReposidory.Instance.Items.Values);

            Initialized += new EventHandler(CachedFilteredComboBox1_Initialized);
            this.DropDownOpened += new EventHandler(CachedFilteredComboBox_DropDownOpened);
        }

        void CachedFilteredComboBox1_Initialized(object sender, EventArgs e)
        {
            if(!IsLazy)
            {
                ensureCache();    
            }
            
        }

        public bool IsLazy { get; set; }

        protected abstract ObjectsCacheBase<T> GetCache();

        void CachedFilteredComboBox_DropDownOpened(object sender, EventArgs e)
        {
            if (queryingPopup || (cache!=null && cache.IsLoaded))
            {
                queryingPopup = false;
                ImageButtonExt.SetIsLoading(this, false);
                return;
            }
            
            
            queryingPopup = true;
            
            //cache = null;
            //loaded = false;
            //Console.WriteLine(cache!=null);
            //Console.WriteLine(loaded);
            ensureCache();
            ImageButtonExt.SetIsLoading(this, !cache.IsLoaded);
            MainWindow.Instance.RunAsynchronousOperation(delegate
            {
                cache.EnsureLoaded();
                //IsDropDownOpen = true;
                //IsEnabled = true;
                //Dispatcher.BeginInvoke(new Action(delegate
                //{
                    //var selectedId = SelectedValue;
                    //ItemsSource = cache.Items.Select(x => x.Value);
                    //SelectedValue = selectedId;
                    //loaded = true;
                    //IsEnabled = true;
                    //IsDropDownOpen = true;
                    
                //}));
            });
            //ImageButtonExt.SetIsLoading(this, !cache.IsLoaded);
        }

        private void ensureCache()
        {
            if (cache == null)
            {
                cache = GetCache();
                CollectionChangedEventManager.AddListener(cache, this);
                if (cache.IsLoaded)
                {
                    //ItemsSource = cache.Items.Values.ToList();
                    ItemsSource = cache.Items.Values.Where(FilterCollection).ToList();
                    EnsureSorting();
                    queryingPopup = false;
                }
            }
        }

        protected virtual void EnsureSorting()
        {
            if (typeof(IHasName).IsAssignableFrom(typeof(T)))
            {
                Items.SortDescriptions.Clear();
                SortDescription sd = new SortDescription("Name", ListSortDirection.Ascending);
                Items.SortDescriptions.Add(sd);
            }
        }

        protected override void OnPreviewKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            if (AllowNull && e.Key == Key.Delete)
            {
                SelectedIndex = -1;
                e.Handled = true;
            }
        }

        protected virtual bool FilterCollection(T item)
        {
            return true;
        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            var notify = (NotifyCollectionChangedEventArgs)e;

            UIHelper.BeginInvoke(delegate
            {
                ImageButtonExt.SetIsLoading(this, false);
                if (notify.Action == NotifyCollectionChangedAction.Reset)
                {
                    ItemsSource = null;
                }
                else
                {
                    var selectedItem = SelectedItem;
                    //ItemsSource = cache.Items.Values.ToList();
                    ItemsSource = cache.Items.Values.Where(FilterCollection).ToList();
                    EnsureSorting();
                    if (selectedItem != null)
                    {
                        SelectedItem = null;
                        SelectedItem = cache.GetItem(((BAGlobalObject)selectedItem).GlobalId);
                    }
                }
            }, Dispatcher);

            return true;
        }

        public static readonly DependencyProperty AllowNullProperty =
            DependencyProperty.Register("AllowNull", typeof(bool), typeof(CachedFilteredComboBox<T>), new UIPropertyMetadata(false));

        private bool queryingPopup;

        public bool AllowNull
        {
            get { return (bool)GetValue(AllowNullProperty); }
            set
            {
                SetValue(AllowNullProperty, value);
            }
        }
    }

    /*
    public class CachedFilteredComboBox<T> : FilteredComboBox, IWeakEventListener where T : BAGlobalObject
    {
        private ObjectsCacheBase<T> cache;


        public CachedFilteredComboBox(ObjectsCacheBase<T> cache)
        {
            this.Style = new Style(GetType(), this.FindResource("LoadingComboBox") as Style);
            CollectionChangedEventManager.AddListener(cache, this);
            this.cache = cache;
            if (cache.IsLoaded)
            {
                ItemsSource = cache.Items.Values.ToList();
                EnsureSorting();
            }

            this.DropDownOpened += new EventHandler(CachedFilteredComboBox_DropDownOpened);
        }


        void CachedFilteredComboBox_DropDownOpened(object sender, EventArgs e)
        {
            ImageButtonExt.SetIsLoading(this, !cache.IsLoaded);
        }

        protected  virtual void EnsureSorting()
        {
            if (typeof(IHasName).IsAssignableFrom(typeof(T)))
            {
                Items.SortDescriptions.Clear();
                SortDescription sd = new SortDescription("Name", ListSortDirection.Ascending);
                Items.SortDescriptions.Add(sd);
            }
        }

        protected override void OnPreviewKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            if (AllowNull && e.Key == Key.Delete)
            {
                SelectedIndex = -1;
                e.Handled = true;
            }
        }


        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            var notify = (NotifyCollectionChangedEventArgs)e;

            UIHelper.BeginInvoke(delegate
                                {
                                    ImageButtonExt.SetIsLoading(this, false);
                                    if (notify.Action == NotifyCollectionChangedAction.Reset)
                                    {
                                        ItemsSource = null;
                                    }
                                    else
                                    {
                                        var selectedItem = SelectedItem;
                                        ItemsSource = cache.Items.Values.ToList();
                                        EnsureSorting();
                                        if (selectedItem != null)
                                        {
                                            SelectedItem = null;
                                            SelectedItem = cache.GetItem(((BAGlobalObject) selectedItem).GlobalId);
                                        }
                                    }
                                }, Dispatcher);
            
            return true;
        }

        public static readonly DependencyProperty AllowNullProperty =
            DependencyProperty.Register("AllowNull", typeof(bool), typeof(CachedFilteredComboBox<T>), new UIPropertyMetadata(false));

        public bool AllowNull
        {
            get { return (bool)GetValue(AllowNullProperty); }
            set
            {
                SetValue(AllowNullProperty, value);
            }
        }
    }
     * */
}
