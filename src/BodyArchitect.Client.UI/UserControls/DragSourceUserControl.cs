using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BodyArchitect.Client.UI.Controls;

namespace BodyArchitect.Client.UI.UserControls
{
    public class DragSourceUserControl:UserControl
    {
        public void Connect(ListBox listBox)
        {
            listBox.PreviewMouseLeftButtonDown += element_PreviewMouseLeftButtonDown;
            listBox.MouseMove += element_MouseMove;
        }
        #region Drag & drop


        public FrameworkElement DropScope
        {
            get { return (FrameworkElement)GetValue(DropScopeProperty); }
            set
            {
                SetValue(DropScopeProperty, value);
            }
        }


        public static readonly DependencyProperty DropScopeProperty =
            DependencyProperty.Register("DropScope", typeof(FrameworkElement), typeof(DragSourceUserControl), new UIPropertyMetadata(null));

        public bool AllowDrag
        {
            get { return (bool)GetValue(AllowDragProperty); }
            set
            {
                SetValue(AllowDragProperty, value);
            }
        }


        public static readonly DependencyProperty AllowDragProperty =
            DependencyProperty.Register("AllowDrag", typeof(bool), typeof(DragSourceUserControl), new UIPropertyMetadata(false));

        DragDropHelper dragDrop = new DragDropHelper();
        private Point startPoint;
        void element_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Store the mouse position
            startPoint = e.GetPosition(null);
        }

        void element_MouseMove(object sender, MouseEventArgs e)
        {
            ListBox parent = (ListBox)sender;
            dragSource = parent;
            Point mousePos = e.GetPosition(null);
            Vector diff = startPoint - mousePos;

            if (AllowDrag && e.LeftButton == MouseButtonState.Pressed && (
        Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
        Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                object data = dragSource.GetDataFromListBox(e.GetPosition(parent));

                if (data != null)
                {
                    var element = dragSource.GetItemFromListBox(e.GetPosition(parent));
                    dragDrop.StartDragDrop(e, element, DropScope, delegate
                    {
                        DataObject dragData = new DataObject(data.GetType().Name, data);
                        DragDrop.DoDragDrop(parent, dragData, DragDropEffects.Move);
                    });

                }
            }
        }


        ListBox dragSource = null;

        #endregion
    }
}
