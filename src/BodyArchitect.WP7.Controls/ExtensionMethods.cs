using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using BodyArchitect.WP7.Controls.Animations;
using Coding4Fun.Phone.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace BodyArchitect.WP7.Controls
{
    public static class ExtensionMethods
    {
        public static ApplicationBar CreateApplicationBar()
        {
            var app = new ApplicationBar();
            app.BackgroundColor = (Color)Application.Current.Resources["CustomApplicationBarBackgroundColor"];
            app.ForegroundColor = (Color)Application.Current.Resources["CustomApplicationBarForegroundColor"];
            return app;
        }

        public static MessagePrompt ShowPopup(this UserControl ctrl, Action<MessagePrompt> messagePrompt = null, DependencyObject page = null)
        {
            MessagePrompt t = new MessagePrompt();
            foreach (var btn in t.ActionPopUpButtons)
            {
                btn.Foreground = (Brush) Application.Current.Resources["CustomForegroundBrush"];
                btn.BorderBrush = (Brush)Application.Current.Resources["CustomForegroundBrush"];
            }
            t.Background = (Brush)Application.Current.Resources["CustomChromeBrush"];
            t.Foreground = (Brush)Application.Current.Resources["CustomForegroundBrush"];
            t.BorderBrush = (Brush)Application.Current.Resources["CustomForegroundBrush"];
            t.IsAppBarVisible = true;
            if (messagePrompt != null)
            {
                messagePrompt(t);
            }
            t.Body = ctrl;
            t.Show();
            if (page != null)
            {
                var anim = page.GetParent<AnimatedBasePage>();
                if (anim != null)
                {
                    anim.ModalDialog = t;
                    t.Unloaded += delegate
                    {
                        anim.ModalDialog = null;
                    };
                }
            }
            return t;
        }

        public static void Invoke(Action action, Dispatcher dispatcher )
        {
            if (dispatcher.CheckAccess())
            {
                // This thread has access so it can update the UI thread.
                action();
            }
            else
            {
                // This thread does not have access to the UI thread.
                // Place the update method on the Dispatcher of the UI thread.
                dispatcher.BeginInvoke(action);
            }
        }

        public static void Navigate(this PhoneApplicationPage page,string uri)
        {
            try
            {
                page.NavigationService.Navigate(new Uri(uri, UriKind.Relative));
            }
            catch (Exception)
            {

            }
            
        }
        public static void GroupViewClose(this LongListSelector list, GroupViewClosingEventArgs e)
        {
            //Cancelling automatic closing and scrolling to do it manually.
            e.Cancel = true;
            if (e.SelectedGroup != null)
            {
                list.ScrollToGroup(e.SelectedGroup);
            }
            
            //Dispatch the swivel animation for performance on the UI thread.
            Application.Current.RootVisual.Dispatcher.BeginInvoke(() =>
            {
                //Construct and begin a swivel animation to pop out the group view.
                IEasingFunction quadraticEase = new QuadraticEase { EasingMode = EasingMode.EaseOut };
                Storyboard _swivelHide = new Storyboard();
                ItemsControl groupItems = e.ItemsControl;

                foreach (var item in groupItems.Items)
                {
                    UIElement container = groupItems.ItemContainerGenerator.ContainerFromItem(item) as UIElement;
                    if (container != null)
                    {
                        Border content = VisualTreeHelper.GetChild(container, 0) as Border;
                        if (content != null)
                        {
                            DoubleAnimationUsingKeyFrames showAnimation = new DoubleAnimationUsingKeyFrames();

                            EasingDoubleKeyFrame showKeyFrame1 = new EasingDoubleKeyFrame();
                            showKeyFrame1.KeyTime = TimeSpan.FromMilliseconds(0);
                            showKeyFrame1.Value = 0;
                            showKeyFrame1.EasingFunction = quadraticEase;

                            EasingDoubleKeyFrame showKeyFrame2 = new EasingDoubleKeyFrame();
                            showKeyFrame2.KeyTime = TimeSpan.FromMilliseconds(125);
                            showKeyFrame2.Value = 90;
                            showKeyFrame2.EasingFunction = quadraticEase;

                            showAnimation.KeyFrames.Add(showKeyFrame1);
                            showAnimation.KeyFrames.Add(showKeyFrame2);

                            Storyboard.SetTargetProperty(showAnimation, new PropertyPath(PlaneProjection.RotationXProperty));
                            Storyboard.SetTarget(showAnimation, content.Projection);

                            _swivelHide.Children.Add(showAnimation);
                        }
                    }
                }

                _swivelHide.Completed += delegate
                {
                    list.CloseGroupView();
                };
                _swivelHide.Begin();

            });
        }

        public static void GroupViewOpen(this LongListSelector list, GroupViewOpenedEventArgs e)
        {
            //Construct and begin a swivel animation to pop in the group view.
            IEasingFunction quadraticEase = new QuadraticEase { EasingMode = EasingMode.EaseOut };
            Storyboard _swivelShow = new Storyboard();
            ItemsControl groupItems = e.ItemsControl;

            foreach (var item in groupItems.Items)
            {
                UIElement container = groupItems.ItemContainerGenerator.ContainerFromItem(item) as UIElement;
                if (container != null)
                {
                    Border content = VisualTreeHelper.GetChild(container, 0) as Border;
                    if (content != null)
                    {
                        DoubleAnimationUsingKeyFrames showAnimation = new DoubleAnimationUsingKeyFrames();

                        EasingDoubleKeyFrame showKeyFrame1 = new EasingDoubleKeyFrame();
                        showKeyFrame1.KeyTime = TimeSpan.FromMilliseconds(0);
                        showKeyFrame1.Value = -60;
                        showKeyFrame1.EasingFunction = quadraticEase;

                        EasingDoubleKeyFrame showKeyFrame2 = new EasingDoubleKeyFrame();
                        showKeyFrame2.KeyTime = TimeSpan.FromMilliseconds(85);
                        showKeyFrame2.Value = 0;
                        showKeyFrame2.EasingFunction = quadraticEase;

                        showAnimation.KeyFrames.Add(showKeyFrame1);
                        showAnimation.KeyFrames.Add(showKeyFrame2);

                        Storyboard.SetTargetProperty(showAnimation, new PropertyPath(PlaneProjection.RotationXProperty));
                        Storyboard.SetTarget(showAnimation, content.Projection);

                        _swivelShow.Children.Add(showAnimation);
                    }
                }
            }

            _swivelShow.Begin();
        }
        /// <summary>
        /// Shows the element, playing a storyboard if one is present
        /// </summary>
        /// <param name="element"></param>
        public static void Show(this FrameworkElement element, Action completedAction)
        {
            string animationName = element.Name + "ShowAnim";

            // check for presence of a show animation
            Storyboard showAnim = element.Resources[animationName] as Storyboard;
            if (showAnim != null)
            {
                showAnim.Begin();
                showAnim.Completed += (s, e) => completedAction();
            }
            else
            {
                element.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Hides the element, playing a storyboard if one is present
        /// </summary>
        /// <param name="element"></param>
        public static void Hide(this FrameworkElement element)
        {
            string animationName = element.Name + "HideAnim";

            // check for presence of a hide animation
            Storyboard showAnim = element.Resources[animationName] as Storyboard;
            if (showAnim != null)
            {
                showAnim.Begin();
            }
            else
            {
                element.Visibility = Visibility.Collapsed;
            }
        }

        public static void MakeFormattedTextBlock(this TextBlock textBlock, string shtml)
        {
            TextBlock tb = textBlock;
            tb.Inlines.Clear();
            tb.TextWrapping = TextWrapping.Wrap;
            Run temprun = new Run();

            int bold = 0;
            int italic = 0;

            do
            {
                if ((shtml.StartsWith("<b>")) | (shtml.StartsWith("<i>")) |
                    (shtml.StartsWith("</b>")) | (shtml.StartsWith("</i>")))
                {
                    bold += (shtml.StartsWith("<b>") ? 1 : 0);
                    italic += (shtml.StartsWith("<i>") ? 1 : 0);
                    bold -= (shtml.StartsWith("</b>") ? 1 : 0);
                    italic -= (shtml.StartsWith("</i>") ? 1 : 0);
                    shtml = shtml.Remove(0, shtml.IndexOf('>') + 1);
                    if (temprun.Text != null)
                        tb.Inlines.Add(temprun);
                    temprun = new Run();
                    temprun.FontWeight = ((bold > 0) ? FontWeights.Bold : FontWeights.Normal);
                    temprun.FontStyle = ((italic > 0) ? FontStyles.Italic : FontStyles.Normal);
                }
                else // just a piece of plain text
                {
                    int nextformatthing = shtml.IndexOf('<');
                    if (nextformatthing < 0) // there isn't any more formatting
                        nextformatthing = shtml.Length;
                    temprun.Text += shtml.Substring(0, nextformatthing);
                    shtml = shtml.Remove(0, nextformatthing);
                }
            } while (shtml.Length > 0);
            // Flush the last buffer
            if (temprun.Text != null)
                tb.Inlines.Add(temprun);
        }

        public static void BindFocusedTextBox()
        {
            TextBox focusTextBox = FocusManager.GetFocusedElement() as TextBox;
            PasswordBox pwdBox = FocusManager.GetFocusedElement() as PasswordBox;
            if (pwdBox != null)
            {
                var binding = pwdBox.GetBindingExpression(PasswordBox.PasswordProperty);
                if (binding != null)
                {
                    binding.UpdateSource();
                }
            }
            if (focusTextBox != null)
            {
                var binding = focusTextBox.GetBindingExpression(TextBox.TextProperty);
                if (binding!=null)
                {
                    binding.UpdateSource();
                }
            }
            
        }

        public static void ShowProgress(this PerformanceProgressBar progressBar,bool show,bool forAppBarAlso=true)
        {
            progressBar.IsIndeterminate = show;
            progressBar.Visibility = show ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            var page = progressBar.GetParent<PhoneApplicationPage>().ApplicationBar;
            if (page != null && forAppBarAlso)
            {
                progressBar.GetParent<PhoneApplicationPage>().ApplicationBar.EnableApplicationBar(!show);
            }
        }

        public static void EnableApplicationBar(this IApplicationBar appBar, bool enable)
        {
            //foreach (IApplicationBarIconButton button in appBar.Buttons)
            //{
            //    button.IsEnabled = enable;
            //}
            appBar.IsVisible = enable;
            appBar.IsMenuEnabled = enable;
        }

        public static T GetParent<T>(this DependencyObject startObject) where T:DependencyObject
        {
            //Walk the visual tree to get the parent(ItemsControl)
            //of this control
            DependencyObject parent = startObject;
            while (parent != null)
            {
                if (parent is T)
                    break;
                else
                    parent = VisualTreeHelper.GetParent(parent);
            }
            return (T)parent;
        }

        public static T FindChild<T>(this DependencyObject parent, string childName=null)
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
                        foundChild = child as T;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = child as T;
                    break;
                }
            }

            return foundChild;
        }

        public static IEnumerable<DependencyObject> Descendents(this DependencyObject root, int depth)
        {
            int count = VisualTreeHelper.GetChildrenCount(root);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(root, i);
                yield return child;
                if (depth > 0)
                {
                    foreach (var descendent in Descendents(child, --depth))
                        yield return descendent;
                }
            }
        }

        public static IEnumerable<DependencyObject> Descendents(this DependencyObject root)
        {
            return Descendents(root, Int32.MaxValue);
        }

        public static IEnumerable<DependencyObject> Ancestors(this DependencyObject root)
        {
            DependencyObject current = VisualTreeHelper.GetParent(root);
            while (current != null)
            {
                yield return current;
                current = VisualTreeHelper.GetParent(current);
            }
        }

    }
}
