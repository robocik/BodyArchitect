using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AvalonDock.Controls;
using AvalonDock.Layout;
using BodyArchitect.Client.Common.Plugins;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.Windows;
using Microsoft.Windows.Controls.Ribbon;

namespace BodyArchitect.Client.UI.UserControls
{
    public static class UserControlRibbon
    {
        private static readonly DependencyProperty RibbonTabBehaviorProperty = DependencyProperty.RegisterAttached(
            "RibbonTabBehavior",
            typeof(RibbonTabBehavior),
            typeof(UserControl));

        private static readonly DependencyProperty RibbonTab1BehaviorProperty = DependencyProperty.RegisterAttached(
            "RibbonTab1Behavior",
            typeof(RibbonTabBehavior),
            typeof(UserControl));

        public static readonly DependencyProperty RibbonTabProperty = DependencyProperty.RegisterAttached(
            "RibbonTab",
            typeof(RibbonTab),
            typeof(UserControlRibbon),
            new PropertyMetadata(null, RibbonTabPropertyChanged));

        public static readonly DependencyProperty RibbonTab1Property = DependencyProperty.RegisterAttached(
            "RibbonTab1",
            typeof(RibbonTab),
            typeof(UserControlRibbon),
            new PropertyMetadata(null, RibbonTab1PropertyChanged));

        public static readonly DependencyProperty RibbonTabDataContextProperty = DependencyProperty.RegisterAttached(
            "RibbonTabDataContext",
            typeof(object),
            typeof(UserControlRibbon),
            new PropertyMetadata(null, RibbonTabDataContextPropertyChanged));

        public static void SetRibbonTab(UserControl control, RibbonTab ribbonTab) { control.SetValue(RibbonTabProperty, ribbonTab); }
        public static RibbonTab GetRibbonTab(UserControl control) { return control.GetValue(RibbonTabProperty) as RibbonTab; }

        public static void SetRibbonTab1(UserControl control, RibbonTab ribbonTab) { control.SetValue(RibbonTab1Property, ribbonTab); }
        public static RibbonTab GetRibbonTab1(UserControl control) { return control.GetValue(RibbonTab1Property) as RibbonTab; }

        public static void RibbonTabPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var target = o as UserControl;
            if (target == null) return;

            var behavior = target.GetValue(RibbonTabBehaviorProperty) as RibbonTabBehavior;
            if (behavior != null)
            {
                behavior.Tab = e.NewValue as RibbonTab;
                return;
            }

            target.SetValue(RibbonTabBehaviorProperty, new RibbonTabBehavior(target, e.NewValue as RibbonTab, target.GetValue(RibbonTabDataContextProperty)));
        }

        public static void RibbonTab1PropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var target = o as UserControl;
            if (target == null) return;

            var behavior = target.GetValue(RibbonTab1BehaviorProperty) as RibbonTabBehavior;
            if (behavior != null)
            {
                behavior.Tab = e.NewValue as RibbonTab;
                return;
            }

            target.SetValue(RibbonTabBehaviorProperty, new RibbonTabBehavior(target, e.NewValue as RibbonTab, target.GetValue(RibbonTabDataContextProperty)));
        }


        public static void SetRibbonTabDataContext(UserControl control, object dataContext)
        {
            control.SetValue(RibbonTabDataContextProperty, dataContext);
        }
        public static object GetRibbonTabDataContext(UserControl control)
        {
            return control.GetValue(RibbonTabDataContextProperty);
        }
        
        public static void RibbonTabDataContextPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var target = o as UserControl;
            if (target == null) return;

            var behavior = target.GetValue(RibbonTabBehaviorProperty) as RibbonTabBehavior;
            if (behavior != null)
                behavior.DataContext = e.NewValue;

            behavior = target.GetValue(RibbonTab1BehaviorProperty) as RibbonTabBehavior;
            if (behavior != null)
                behavior.DataContext = e.NewValue;
        }
    }

    public class RibbonTabBehavior
    {
        public RibbonTabBehavior(UserControl control, RibbonTab tab, object dataContext)
        {
            this.tab = tab;
            Control = control;
            this.dataContext = dataContext;
            control.IsVisibleChanged += ControlVisibilityChanged;
            //Control.Loaded += Control_Loaded;
            
        }

        void dockManager_ActiveContentChanged(object sender, EventArgs e)
        {
            var layoutDocuments = MainWindow.Instance.GetCurrentDocument();
            if (layoutDocuments!=null && layoutDocuments.Content == UIHelper.FindVisualParent<Frame>(Control))
            {
                AttachTabToRibbon();
            }
            else
            {
                DetachTabFromRibbon();
            }
        }


        private void ControlVisibilityChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Tab == null || Control == null)
                return;
            if ((bool)e.NewValue == true)
            {
                MainWindow.Instance.dockManager.ActiveContentChanged -= dockManager_ActiveContentChanged;
                MainWindow.Instance.dockManager.ActiveContentChanged += dockManager_ActiveContentChanged;
                AttachTabToRibbon();
            }
            else
            {
                MainWindow.Instance.dockManager.ActiveContentChanged -= dockManager_ActiveContentChanged;
                DetachTabFromRibbon();
            }
        }



        private void AttachTabToRibbon()
        {
            if (Ribbon == null)
                GetRibbon();
            if (Ribbon == null)
                return;

            var layoutDocuments = MainWindow.Instance.GetCurrentDocument();
            if (layoutDocuments==null || layoutDocuments.Content != UIHelper.FindVisualParent<Frame>(Control))
            {
                return;
            }


            if (!string.IsNullOrEmpty(Tab.Uid) && !(Control is IControlView))
            {
                var tabs=Ribbon.Items.Cast<RibbonTab>().Where(x => x.Uid == Tab.Uid).ToList();
                RibbonTab existingTab = null;
                if (tabs.Count > 1)
                {//very special case (we have opened two strength trainings side by side
                    existingTab=tabs.Where(x => x.Tag != null && UIHelper.FindVisualParent<IControlView>(Control) != x.Tag).SingleOrDefault();
                }
                if (existingTab == null)
                {
                    existingTab = tabs.FirstOrDefault();
                }
                if (existingTab != null && existingTab != Tab)
                {
                    List<RibbonGroup> detachedGroups = new List<RibbonGroup>();
                    Tab.Tag = detachedGroups;
                    for (int i = Tab.Items.Count-1; i >=0; i--)
                    {
                        var item =(RibbonGroup) Tab.Items[i];
                        item.Tag =new Tuple<RibbonTab,Control>(Tab,Control);
                        item.Uid = Tab.Uid;
                        Tab.Items.RemoveAt(i);
                        detachedGroups.Add(item);
                        item.DataContext = DataContext;
                        existingTab.Items.Add(item);
                        
                    }
                    return;
                }
            }

            if (Tab.Parent != null)
            {
                return;
            }
            Tab.Tag = Control;
            Ribbon.Items.Add(Tab);
            Tab.DataContext = DataContext;

            foreach(var creator in PluginsManager.Instance.RibbonCreators)
            {
                creator.CreateRibbonGroup(Tab);
            }

            if (Tab.Visibility == Visibility.Visible)
            {
                Ribbon.SelectedItem = Tab;
            }

        }

        private void GetRibbon()
        {
            var control = VisualTreeHelper.GetParent(Control);
            Ribbon = null;
            while (control != null && Ribbon == null)
            {
                var numberOfChildren = VisualTreeHelper.GetChildrenCount(control);
                for (int i = 0; i < numberOfChildren; ++i)
                {
                    var child = VisualTreeHelper.GetChild(control, i);
                    if (child is Ribbon)
                    {
                        Ribbon = child as Ribbon;
                        return;
                    }
                }
                control = VisualTreeHelper.GetParent(control);
            }
        }

        private void DetachTabFromRibbon()
        {
            if (Ribbon == null || Tab == null)
                return;



            if (!string.IsNullOrEmpty(Tab.Uid) && !(Control is IControlView))
            {
                //existingTab=tabs.Where(x => x.Tag != null && UIHelper.FindVisualParent<IControlView>(Control) != x.Tag).SingleOrDefault();
                //var existingTab = Ribbon.Items.Cast<RibbonTab>().Where(x => x.Uid == Tab.Uid && UIHelper.FindVisualParent<IControlView>(Control) == x.Tag).SingleOrDefault();

                var existingTab = Ribbon.Items.Cast<RibbonTab>().Where(x => x.Uid == Tab.Uid).SingleOrDefault();

                if (existingTab != null)
                {
                    //for (int i = existingTab.Items.Count - 1; i >= 0; i--)
                    //{
                    //    var item = (RibbonGroup)existingTab.Items[i];
                    //    if (item.Uid == Tab.Uid)
                    //    {
                    //        existingTab.Items.RemoveAt(i);
                    //        RibbonTab originalOwnerTab = item.Tag as RibbonTab;
                    //        if (originalOwnerTab == null)
                    //        {
                    //            originalOwnerTab = Tab;
                    //        }
                    //        originalOwnerTab.Items.Add(item);
                    //        originalOwnerTab.Tag = null;
                    //        // item.DataContext = null;

                    //    }
                    //}
                    for (int i = existingTab.Items.Count - 1; i >= 0; i--)
                    {
                        var item = (RibbonGroup)existingTab.Items[i];
                        Tuple<RibbonTab, Control> tupple = (Tuple<RibbonTab, Control>) item.Tag;
                        if (item.Uid == Tab.Uid && tupple.Item2== Control)
                        {
                            existingTab.Items.RemoveAt(i);
                            RibbonTab originalOwnerTab = tupple.Item1;
                            if (originalOwnerTab == null)
                            {
                                originalOwnerTab = Tab;
                            }
                            originalOwnerTab.Items.Add(item);
                            originalOwnerTab.Tag = null;
                        }
                    }
                }
            }

            Ribbon.Items.Remove(Tab);
        }

        private RibbonTab tab;
        public RibbonTab Tab
        {
            get { return tab; }
            set
            {
                if (tab != null)
                    DetachTabFromRibbon();
                tab = value;
                if (tab != null)
                    AttachTabToRibbon();
            }
        }

        private object dataContext;
        public object DataContext
        {
            get { return dataContext; }
            set
            {
                dataContext = value;
                if (Tab != null)
                {
                    Tab.DataContext = value;
                    var detachedItems = Tab.Tag as List<RibbonGroup>;
                    if (detachedItems==null)
                    {
                        return;
                    }
                    foreach (var detachedItem in detachedItems)
                    {
                        detachedItem.DataContext = value;
                    }
                }
            }
        }

        public UserControl Control { get; set; }
        public Ribbon Ribbon { get; set; }
    }
}
