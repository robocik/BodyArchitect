using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace BodyArchitect.WP7.Controls.Animations
{
    public static class ListAnimation
    {
        #region AnimationLevel

        public static int GetAnimationLevel(DependencyObject obj)
        {
            return (int)obj.GetValue(AnimationLevelProperty);
        }

        public static void SetAnimationLevel(DependencyObject obj, int value)
        {
            obj.SetValue(AnimationLevelProperty, value);
        }


        public static readonly DependencyProperty AnimationLevelProperty =
            DependencyProperty.RegisterAttached("AnimationLevel", typeof(int),
            typeof(ListAnimation), new PropertyMetadata(-1));

        #endregion

        #region IsPivotAnimated

        public static bool GetIsPivotAnimated(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsPivotAnimatedProperty);
        }

        public static void SetIsPivotAnimated(DependencyObject obj, bool value)
        {
            obj.SetValue(IsPivotAnimatedProperty, value);
        }

        public static readonly DependencyProperty IsPivotAnimatedProperty =
            DependencyProperty.RegisterAttached("IsPivotAnimated", typeof(bool),
            typeof(ListAnimation), new PropertyMetadata(false, OnIsPivotAnimatedChanged));

        private static void OnIsPivotAnimatedChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ItemsControl list = d as ItemsControl;

            list.Loaded += (s2, e2) =>
            {
                // locate the pivot control that this list is within
                Pivot pivot = list.Ancestors<Pivot>().Single() as Pivot;

                // and its index within the pivot
                int pivotIndex = pivot.Items.IndexOf(list.Ancestors<PivotItem>().Single());

                bool selectionChanged = false;

                pivot.SelectionChanged += (s3, e3) =>
                {
                    selectionChanged = true;
                };

                // handle manipulation events which occur when the user
                // moves between pivot items
                pivot.ManipulationCompleted += (s, e) =>
                {
                    if (!selectionChanged)
                        return;

                    selectionChanged = false;

                    if (pivotIndex != pivot.SelectedIndex)
                        return;

                    // determine which direction this tab will be scrolling in from
                    bool fromRight = e.TotalManipulation.Translation.X <= 0;

                    // locate the stack panel that hosts the items
                    VirtualizingStackPanel vsp = list.Descendants<VirtualizingStackPanel>().First() as VirtualizingStackPanel;

                    // iterate over each of the items in view
                    int firstVisibleItem = (int)vsp.VerticalOffset;
                    int visibleItemCount = (int)vsp.ViewportHeight;
                    for (int index = firstVisibleItem; index <= firstVisibleItem + visibleItemCount; index++)
                    {
                        // find all the item that have the AnimationLevel attached property set
                        var lbi = list.ItemContainerGenerator.ContainerFromIndex(index);
                        if (lbi == null)
                            continue;

                        vsp.Dispatcher.BeginInvoke(() =>
                        {
                            var animationTargets = lbi.Descendants().Where(p => ListAnimation.GetAnimationLevel(p) > -1);
                            foreach (FrameworkElement target in animationTargets)
                            {
                                // trigger the required animation
                                GetAnimation(target, fromRight).Begin();
                            }
                        });
                    };
                };
            };
        }


        #endregion


        /// <summary>
        /// Creates a TranslateTransform and associates it with the given element, returning
        /// a Storyboard which will animate the TranslateTransform with a SineEase function
        /// </summary>
        private static Storyboard GetAnimation(FrameworkElement element, bool fromRight)
        {
            double from = fromRight ? 80 : -80;

            Storyboard sb;
            double delay = (ListAnimation.GetAnimationLevel(element)) * 0.1 + 0.1;

            TranslateTransform trans = new TranslateTransform() { X = from };
            element.RenderTransform = trans;

            sb = new Storyboard();
            sb.BeginTime = TimeSpan.FromSeconds(delay);

            DoubleAnimation db = new DoubleAnimation();
            db.To = 0;
            db.From = from;
            db.EasingFunction = new SineEase();
            sb.Duration = db.Duration = TimeSpan.FromSeconds(0.8);
            sb.Children.Add(db);
            Storyboard.SetTarget(db, trans);
            Storyboard.SetTargetProperty(db, new PropertyPath("X"));

            return sb;
        }

        /// <summary>
        /// Creates a PlaneProjection and associates it with the given element, returning
        /// a Storyboard which will animate the PlaneProjection to 'peel' the item
        /// from the screen.
        /// </summary>
        private static Storyboard GetPeelAnimation(FrameworkElement element, double delay)
        {
            Storyboard sb;

            var projection = new PlaneProjection()
            {
                CenterOfRotationX = -0.1
            };
            element.Projection = projection;

            // compute the angle of rotation required to make this element appear
            // at a 90 degree angle at the edge of the screen.
            var width = element.ActualWidth;
            var targetAngle = Math.Atan(1000 / (width / 2));
            targetAngle = targetAngle * 180 / Math.PI;

            // animate the projection
            sb = new Storyboard();
            sb.BeginTime = TimeSpan.FromMilliseconds(delay);
            sb.Children.Add(CreateAnimation(0, -(180 - targetAngle), 0.3, "RotationY", projection));
            sb.Children.Add(CreateAnimation(0, 23, 0.3, "RotationZ", projection));
            sb.Children.Add(CreateAnimation(0, -23, 0.3, "GlobalOffsetZ", projection));
            return sb;
        }

        /// <summary>
        /// Enumerates all the items that are currently visible in am ItemsControl. This implementation assumes
        /// that a VirtualizingStackPanel is being used as the ItemsPanel.
        /// </summary>
        public static IEnumerable<FrameworkElement> GetItemsInView(this ItemsControl itemsControl)
        {
            // locate the stack panel that hosts the items
            VirtualizingStackPanel vsp = itemsControl.Descendants<VirtualizingStackPanel>().First() as VirtualizingStackPanel;

            // iterate over each of the items in view
            int firstVisibleItem = (int)vsp.VerticalOffset;
            int visibleItemCount = (int)vsp.ViewportHeight;
            for (int index = firstVisibleItem; index <= firstVisibleItem + visibleItemCount + 1; index++)
            {
                var item = itemsControl.ItemContainerGenerator.ContainerFromIndex(index) as FrameworkElement;
                if (item == null)
                    continue;

                yield return item;
            }
        }

        private static DoubleAnimation CreateAnimation(double from, double to, double duration,
          string targetProperty, DependencyObject target)
        {
            var db = new DoubleAnimation();
            db.To = to;
            db.From = from;
            db.EasingFunction = new SineEase();
            db.Duration = TimeSpan.FromSeconds(duration);
            Storyboard.SetTarget(db, target);
            Storyboard.SetTargetProperty(db, new PropertyPath(targetProperty));
            return db;
        }

        /// <summary>
        /// Animates each element in order, creating a 'peel' effect. The supplied action
        /// is invoked when the animation ends.
        /// </summary>
        public static void Peel(this IEnumerable<FrameworkElement> elements, Action endAction)
        {
            var elementList = elements.ToList();
            var lastElement = elementList.Last();

            // iterate over all the elements, animating each of them
            double delay = 0;
            foreach (FrameworkElement element in elementList)
            {
                var sb = GetPeelAnimation(element, delay);

                // add a Completed event handler to the last element
                if (element.Equals(lastElement))
                {
                    sb.Completed += (s, e) =>
                    {
                        endAction();
                    };
                }

                sb.Begin();
                delay += 50;
            }
        }

    }
}
