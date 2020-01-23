using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Logger;
using BodyArchitect.Portable.Exceptions;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Resources.Localization;
using System.ServiceModel;

namespace BodyArchitect.Client.UI.Controls
{
    public static class UIHelper
    {
        public static MainWindow MainWindow { get; set; }

        public static void Collapse(this RowDefinition row,bool collapse)
        {
            if (collapse)
            {
                row.Height = new GridLength(1, GridUnitType.Star);
            }
            else
            {
                row.Height = new GridLength(0, GridUnitType.Pixel);
            }
        }

        public static bool EnsureInstructorLicence()
        {
            if (!UserContext.IsInstructor)
            {
                ChangeAccountTypeWindow dlg = new ChangeAccountTypeWindow(false);
                if (dlg.ShowDialog() == true)
                {
                    MainWindow.Instance.ShowPage(new Uri("pack://application:,,,/BodyArchitect.Client.UI;component/Views/ChangeAccountTypeView.xaml"), showInNewTab: true);
                }
                return false;
            }
            return true;
        }

        public static bool EnsurePremiumLicence()
        {
            if (!UserContext.IsPremium)
            {
                ChangeAccountTypeWindow dlg = new ChangeAccountTypeWindow(true);
                if (dlg.ShowDialog() == true)
                {
                    MainWindow.Instance.ShowPage(new Uri("pack://application:,,,/BodyArchitect.Client.UI;component/Views/ChangeAccountTypeView.xaml"), showInNewTab: true);
                }
                return false;
            }
            return true;
        }

        public static void DoEvents()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,new Action(delegate { }));
        }

        public static BitmapImage ToBitmap(this string uri)
        {
            return new Uri(uri,UriKind.Absolute).ToBitmap();
        }

        public static BitmapImage ToBitmap(this Uri uri)
        {
            BitmapImage source = new BitmapImage(uri);
            source.Freeze();
            return source;
        }
        public static void Refresh(this ItemsControl list)
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(list.ItemsSource);
            view.Refresh();
        }

        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }

        /// <summary>
        /// Finds a Child of a given item in the visual tree. 
        /// </summary>
        /// <param name="parent">A direct parent of the queried item.</param>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="childName">x:Name or Name of child. </param>
        /// <returns>The first parent item that matches the submitted type parameter. 
        /// If not matching item can be found, 
        /// a null parent is being returned.</returns>
        public static T FindChild<T>(DependencyObject parent, string childName)
           where T : class 
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild =child as T;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild =child as T;
                    break;
                }
            }

            return foundChild;
        }

        /// <summary>
        /// Finds a Child of a given item in the visual tree. 
        /// </summary>
        /// <param name="parent">A direct parent of the queried item.</param>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <returns>The first parent item that matches the submitted type parameter. 
        /// If not matching item can be found, 
        /// a null parent is being returned.</returns>
        public static List<T> FindChild<T>(DependencyObject parent)
           where T : class
        {
            List<T> result = new List<T>();
            // Confirm parent and childName are valid. 
            if (parent == null) return null;


            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType != null)
                {
                    result.Add(childType);
                }

                // recursively drill down the tree
                result.AddRange(FindChild<T>(child));
            }

            return result;
        }

        /// <summary>
        /// Finds a parent of a given item on the visual tree.
        /// </summary>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="child">A direct or indirect child of the queried item.</param>
        /// <returns>The first parent item that matches the submitted type parameter. 
        /// If not matching item can be found, a null reference is being returned.</returns>
        public static T FindVisualParent<T>(this DependencyObject child)
          where T : class
        {
            // get parent item
            var parentObject = VisualTreeHelper.GetParent(child);
            // we’ve reached the end of the tree
            if (parentObject == null)
            {
                FrameworkElement frameworkElement = child as FrameworkElement;
                if (frameworkElement == null)
                    return null;
                parentObject = frameworkElement.Parent;
            }
            if(parentObject==null)
            {
                return null;
            }

            // check if the parent matches the type we’re looking for
            T parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                // use recursion to proceed with next level
                return FindVisualParent<T>(parentObject);
            }
        }

        public static void SetVisible(this FrameworkElement control, bool isVisible)
        {
            control.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
        }


        public static object GetClickedItem(this ItemsControl listbox, MouseButtonEventArgs e)
        {
            UIElement elem = (UIElement)listbox.InputHitTest(e.GetPosition(listbox));
            while (elem != listbox)
            {
                if (elem is ListBoxItem)
                {
                    object selectedItem = ((ListBoxItem)elem).Content;
                    // Handle the double click here
                    return selectedItem;
                }
                elem = (UIElement)VisualTreeHelper.GetParent(elem);
            }
            return null;
        }

        public static void BeginInvoke(Action action,Dispatcher dispatcher)
        {
            if (dispatcher==null)
            {
                dispatcher = Dispatcher.CurrentDispatcher;
            }
            if (dispatcher.CheckAccess())
            {
                // This thread has access so it can update the UI thread.
                action();
            }
            else
            {
                // This thread does not have access to the UI thread.
                // Place the update method on the Dispatcher of the UI thread.
                dispatcher.BeginInvoke(DispatcherPriority.Normal, action);
            }
        }

        public static void Invoke(Action action, Dispatcher dispatcher)
        {
            if (dispatcher == null)
            {
                dispatcher = Dispatcher.CurrentDispatcher;
            }
            if (dispatcher.CheckAccess())
            {
                // This thread has access so it can update the UI thread.
                action();
            }
            else
            {
                // This thread does not have access to the UI thread.
                // Place the update method on the Dispatcher of the UI thread.
                dispatcher.Invoke(DispatcherPriority.Normal, action);
            }
        }

        public static FrameworkElement GetItemFromListBox(this ListBox source, Point point)
        {
            UIElement tmp = source.InputHitTest(point) as UIElement;
            UIElement element = null;
            if (tmp != null)
            {
                while (tmp.GetType() != typeof(ContentPresenter))
                {

                    if (tmp.GetType() != typeof(ContentPresenter))
                    {
                        element = tmp;
                    }
                    tmp = VisualTreeHelper.GetParent(element) as UIElement;
                }
                return element as FrameworkElement;
            }
            return null;
        }

        public static object GetDataFromListBox(this ListBox source, Point point)
        {
            UIElement element = source.InputHitTest(point) as UIElement;
            if (element != null)
            {
                object data = DependencyProperty.UnsetValue;
                while (data == DependencyProperty.UnsetValue)
                {
                    data = source.ItemContainerGenerator.ItemFromContainer(element);
                    if (data == DependencyProperty.UnsetValue)
                    {
                        element = VisualTreeHelper.GetParent(element) as UIElement;
                    }
                    if (element == source)
                    {
                        return null;
                    }
                }
                if (data != DependencyProperty.UnsetValue)
                {
                    return data;
                }
            }
            return null;
        }

        public static bool RunWithExceptionHandling(Action action,Dispatcher dispatcher=null)
        {
            try
            {
                action();
                return true;
            }
            catch (OperationCanceledException)
            {

            }
            catch (OldDataException ex)
            {
                Invoke(() => ExceptionHandler.Default.Process(ex, Strings.ErrorOldDataModification, ErrorWindow.MessageBox),dispatcher);
            }
            catch (ProfileDeletedException ex)
            {
                Invoke(() => ExceptionHandler.Default.Process(ex, Strings.ErrorCurrentProfileDeleted, ErrorWindow.MessageBox), dispatcher);
                UserContext.Current.Logout(resetAutologon: false, skipLogoutOnServer: true);
            }
            catch (UserDeletedException ex)
            {
                Invoke(() => ExceptionHandler.Default.Process(ex, Strings.ErrorProfileDeleted, ErrorWindow.MessageBox), dispatcher);
            }
            catch (ValidationException ex)
            {
                Invoke(() => BAMessageBox.ShowValidationError(ex.Results), dispatcher);
            }
            catch (EndpointNotFoundException ex)
            {
                Invoke(() => ExceptionHandler.Default.Process(ex, Strings.ErrorConnectionProblem, ErrorWindow.MessageBox), dispatcher);
                UserContext.Current.Logout(resetAutologon: false, skipLogoutOnServer: true);
            }
            catch (LicenceException ex)
            {
                Invoke(() => ExceptionHandler.Default.Process(ex, Strings.ErrorLicence, ErrorWindow.MessageBox), dispatcher);
            }
            catch (TimeoutException ex)
            {
                Invoke(() => ExceptionHandler.Default.Process(ex, Strings.ErrorConnectionProblem, ErrorWindow.MessageBox), dispatcher);
                UserContext.Current.Logout(resetAutologon: false, skipLogoutOnServer: true);
            }
            catch (DatabaseVersionException ex)
            {
                Invoke(() => ExceptionHandler.Default.Process(ex, Strings.ErrorOldVersionOfBodyArchitect, ErrorWindow.MessageBox), dispatcher);
                UserContext.Current.Logout(resetAutologon: false, skipLogoutOnServer: true);
            }
            catch (MaintenanceException ex)
            {
                Invoke(() => ExceptionHandler.Default.Process(ex, Strings.ErrorMaintenanceMode, ErrorWindow.MessageBox), dispatcher);
            }
            catch (Exception ex)
            {
                Invoke(() => ExceptionHandler.Default.Process(ex, ex.Message, ErrorWindow.EMailReport), dispatcher);
            }
            return false;
        }

        public static string DistanceType
        {
            get
            {
                if (UserContext.Current.ProfileInformation.Settings.LengthType == Service.V2.Model.LengthType.Inchs)
                {
                    return "mi";
                }
                return "km";
            }
        }

        public static string TemperatureType
        {
            get
            {
                if (UserContext.Current.ProfileInformation.Settings.LengthType == Service.V2.Model.LengthType.Inchs)
                {
                    return "°F";
                }
                return "°C";
            }
        }


        public static string SpeedType
        {
            get
            {
                if (UserContext.Current.ProfileInformation.Settings.LengthType == Service.V2.Model.LengthType.Inchs)
                {
                    return "mph";
                }
                return "km/h";
            }
        }
        public static string PaceType
        {
            get
            {
                if (UserContext.Current.ProfileInformation.Settings.LengthType == Service.V2.Model.LengthType.Inchs)
                {
                    return "min/mi";
                }
                return "min/km";
            }
        }

        public static string AltitudeType
        {
            get
            {
                if (UserContext.Current.ProfileInformation.Settings.LengthType == Service.V2.Model.LengthType.Inchs)
                {
                    return "ft";
                }
                return "m";
            }
        }

        public static string WeightType
        {
            get
            {
                if (UserContext.Current.ProfileInformation.Settings.WeightType == Service.V2.Model.WeightType.Pounds)
                {
                    return Strings.WeightType_Pound;
                }
                return Strings.WeightType_Kg;
            }
        }

        public static string LengthType
        {
            get
            {
                if (UserContext.Current.ProfileInformation.Settings.LengthType == Service.V2.Model.LengthType.Inchs)
                {
                    return Strings.LengthType_Inch;
                }
                return Strings.LengthType_Cm;
            }
        }
    }
}
