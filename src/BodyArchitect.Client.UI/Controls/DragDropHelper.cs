using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using BodyArchitect.Client.UI.Controls.Calendar.Controls;

namespace BodyArchitect.Client.UI.Controls
{
    public class DragDropHelper
    {
        private Point startPoint;
        private FrameworkElement element;
        private FrameworkElement dragScope;
        private Action dragStart;

        public void SetDragSource(FrameworkElement element, FrameworkElement dragScope, Action dragStart)
        {
            this.element = element;
            this.dragStart = dragStart;
            this.dragScope = dragScope;
            element.MouseMove += new System.Windows.Input.MouseEventHandler(element_MouseMove);
            element.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(element_PreviewMouseLeftButtonDown);
        }

        void element_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Store the mouse position
            startPoint = e.GetPosition(null);
        }

        void element_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Point mousePos = e.GetPosition(null);
            Vector diff = startPoint - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed && (
        Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
        Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                //StartDragDrop();
                StartDragInProcAdorner(e);
            }
        }

        FrameworkElement _dragScope;
        public FrameworkElement DragScope
        {
            get { return _dragScope; }
            set { _dragScope = value; }
        }

        private bool _isDragging;

        public bool IsDragging
        {
            get { return _isDragging; }
            set { _isDragging = value; }
        }
        DragAdorner _adorner = null;
        AdornerLayer _layer;

        private bool _dragHasLeftScope = false;
        void DragScope_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (this._dragHasLeftScope)
            {
                e.Action = DragAction.Cancel;
                e.Handled = true;
            }

        }

        void Window1_DragOver(object sender, DragEventArgs args)
        {
            if (_adorner != null)
            {
                _adorner.LeftOffset = args.GetPosition(DragScope).X /* - _startPoint.X */ ;
                _adorner.TopOffset = args.GetPosition(DragScope).Y /* - _startPoint.Y */ ;
            }
        }

        void DragScope_DragLeave(object sender, DragEventArgs e)
        {
            if (e.OriginalSource == DragScope)
            {
                Point p = e.GetPosition(DragScope);
                Rect r = VisualTreeHelper.GetContentBounds(DragScope);
                if (!r.Contains(p))
                {
                    this._dragHasLeftScope = true;
                    e.Handled = true;
                }
            }

        }

        public void StartDragDrop(MouseEventArgs e, FrameworkElement element, FrameworkElement dragScope, Action dragStart)
        {
            this.element = element;
            this.dragStart = dragStart;
            this.dragScope = dragScope;
            StartDragInProcAdorner(e);
        }

        private void StartDragInProcAdorner(MouseEventArgs e)
        {

            // Let's define our DragScope .. In this case it is every thing inside our main window .. 
            //DragScope = Application.Current.MainWindow.Content as FrameworkElement;
            DragScope = dragScope;

            // We enable Drag & Drop in our scope ...  We are not implementing Drop, so it is OK, but this allows us to get DragOver 
            //bool previousDrop = DragScope.AllowDrop;
            //DragScope.AllowDrop = true;

            // Let's wire our usual events.. 
            // GiveFeedback just tells it to use no standard cursors..  


            //this.DragSource.GiveFeedback += feedbackhandler;

            // The DragOver event ... 
            DragEventHandler draghandler = new DragEventHandler(Window1_DragOver);
            DragScope.PreviewDragOver += draghandler;

            // Drag Leave is optional, but write up explains why I like it .. 
            DragEventHandler dragleavehandler = new DragEventHandler(DragScope_DragLeave);
            DragScope.DragLeave += dragleavehandler;

            // QueryContinue Drag goes with drag leave... 
            QueryContinueDragEventHandler queryhandler = new QueryContinueDragEventHandler(DragScope_QueryContinueDrag);
            DragScope.QueryContinueDrag += queryhandler;

            //Here we create our adorner.. 
            _adorner = new DragAdorner(DragScope, element, true, 0.5);
            _layer = AdornerLayer.GetAdornerLayer(DragScope as Visual);
            _layer.Add(_adorner);


            IsDragging = true;
            _dragHasLeftScope = false;
            //Finally lets drag drop 
            //DataObject data = new DataObject(System.Windows.DataFormats.Text.ToString(), "abcd");
            //DragDropEffects de = DragDrop.DoDragDrop(this.DragSource, data, DragDropEffects.Move);
            //StartDragDrop();
            dragStart();

            // Clean up our mess :) 
            //DragScope.AllowDrop = previousDrop;
            AdornerLayer.GetAdornerLayer(DragScope).Remove(_adorner);
            _adorner = null;

            DragScope.DragLeave -= dragleavehandler;
            DragScope.QueryContinueDrag -= queryhandler;
            DragScope.PreviewDragOver -= draghandler;

            IsDragging = false;
        }

    }
}
