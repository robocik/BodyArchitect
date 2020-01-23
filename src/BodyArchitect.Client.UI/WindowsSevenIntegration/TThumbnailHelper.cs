using System;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using AvalonDock;
using AvalonDock.Layout;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.Windows;
using Microsoft.WindowsAPICodePack.Taskbar;
using Application = System.Windows.Application;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;

namespace BodyArchitect.Client.UI.WindowsSevenIntegration
{
    //TODO: Calculate thumbnail offset (remembering correct offset for LayoutDocumentPane?? )
    //TODO: Floating window preview
    //TODO: Reordering all previews
    //TODO: Refreshing all previews (should work with correct offset)
    //TODO: Check if frames for removing problems (ie memory usage)

    public static class TThumbnailHelper
    {
        #region Fields and properties

        private static Window ParentWindow { get; set; }
        private static DockingManager dockManager { get; set; }
        private static bool _initialized = false;

        //TODO: Change timespan for delays if needed
        private static DispatcherTimer resizeTimer = new DispatcherTimer()
                                                        {
                                                            IsEnabled = false,
                                                            Interval = new TimeSpan(0, 0, 0, 0, 500)
                                                        };

        private static DispatcherTimer moveTimer = new DispatcherTimer()
                                                        {
                                                            IsEnabled = false,
                                                            Interval = new TimeSpan(0, 0, 0, 1, 0)
                                                        };

        private static DispatcherTimer previewCreatorDelay = new DispatcherTimer()
                                                        {
                                                            IsEnabled = false,
                                                            Interval = new TimeSpan(0, 0, 0, 0, 500)
                                                        };
        
        #endregion

        #region Methods

        public static void Initialize(Window mainWnd, LayoutDocumentPane layoutDocumentPane, DockingManager dockingManager)
        {
            if (!_initialized && TaskbarManager.IsPlatformSupported)
            {
                ParentWindow = mainWnd;
                //LayoutPane = layoutDocumentPane;
                dockManager = dockingManager;
                dockManager.ActiveContentChanged += dockManager_ActiveContentChanged;
                //LayoutPane.Children.CollectionChanged += LayoutPane_CollectionChanged;
                //dockManager.Layout.FloatingWindows.CollectionChanged += new NotifyCollectionChangedEventHandler(FloatingWindows_CollectionChanged);
                //Used to avoid too much of refreshing preview bitmap. It prevents window resizing lag
                resizeTimer.Tick += ResizingDone;
                moveTimer.Tick += MovingDone;
                previewCreatorDelay.Tick += WaitingForRefreshDone;

                dockManager.Layout.ElementAdded += new EventHandler<LayoutElementEventArgs>(Layout_ElementAdded);
                dockManager.Layout.ElementRemoved += new EventHandler<LayoutElementEventArgs>(Layout_ElementRemoved);

                ParentWindow.SizeChanged += ParentWindow_SizeStateChanged;
                ParentWindow.StateChanged += ParentWindow_SizeStateChanged;

                //Add ribbon event handlers to recalculate preview offset
                var ribbon = ((MainWindow) ParentWindow).Ribbon;
                ribbon.Collapsed += ParentWindow_RibbonStateChange;
                ribbon.Expanded += ParentWindow_RibbonStateChange;
                ribbon.SizeChanged += ParentWindow_RibbonStateChange;
                _initialized = true;
                
            }
        }
        
        private static void CreateContentPreview(UIElement content)
        {
            if(!_initialized) throw new Exception("Not initialized");
            
            //if (_previewOffset.X == 0.0 && _previewOffset.Y == 0.0) //probably not calculated yet
            //    _previewOffset = SetThumbnailOffset((Frame) content);
            var offset = SetThumbnailOffset((Frame) content);
            TabbedThumbnail thumbnail = new TabbedThumbnail(ParentWindow, content, offset);
            if (!TaskbarManager.Instance.TabbedThumbnail.IsThumbnailPreviewAdded(thumbnail)) //only once
            {
                thumbnail.TabbedThumbnailActivated += new EventHandler<TabbedThumbnailEventArgs>(thumbnail_TabbedThumbnailActivated);
                thumbnail.TabbedThumbnailClosed += new EventHandler<TabbedThumbnailClosedEventArgs>(thumbnail_TabbedThumbnailClosed);
                thumbnail.TabbedThumbnailMaximized += new EventHandler<TabbedThumbnailEventArgs>(thumbnail_TabbedThumbnailMaximized);
                thumbnail.TabbedThumbnailMinimized += new EventHandler<TabbedThumbnailEventArgs>(thumbnail_TabbedThumbnailMinimized);
                thumbnail.DisplayFrameAroundBitmap = false;
                thumbnail.ClippingRectangle = new Rectangle(100, 100, 100, 100);
                //NOTE: ADDING TOOLBAR BUTTON, DOES NOT WORK WHILE DEBUGGING. USUALLY.
                TaskbarManager.Instance.TabbedThumbnail.AddThumbnailPreview(thumbnail);
                ThumbnailButtonsHelper.AddTaskbarButtons(content);
                ThumbnailButtonsHelper.UpdateButtonStatus((Frame)content);
                //Called when using history (it fires even when tab is not active, frame_contentRendered doesn't)
                ((Frame)content).LoadCompleted += (sender, args) =>
                                                        {
                                                            var frame = sender as Frame;
                                                            RefreshPreview(frame);
                                                            ReorderAllPreviews();
                                                            ThumbnailButtonsHelper.UpdateButtonStatus(frame);
                                                        };
            }
            
        }

        private static void SetThumbnailTitleAndIcon(UIElement content, IControlView activeView)
        {
            if (!_initialized) throw new Exception("Not initialized");
            TabbedThumbnail thumbnail = TaskbarManager.Instance.TabbedThumbnail.GetThumbnailPreview(content);
            if (thumbnail != null && activeView != null)
            {
                if (thumbnail.Title != activeView.Header)  //Title is not set yet or sth has changed
                {
                    thumbnail.Title = activeView.Header;
                    thumbnail.Tooltip = activeView.HeaderToolTip;
                    var streamResourceInfo = Application.GetResourceStream(activeView.HeaderIcon);
                    if (streamResourceInfo != null)
                    {
                        Stream iconStream = streamResourceInfo.Stream;
                        var bitmap = new Bitmap(iconStream);
                        var iconHandle = bitmap.GetHicon();
                        var icon = Icon.FromHandle(iconHandle);
                        thumbnail.SetWindowIcon(icon);
                    }
                    RefreshPreview((Frame) content);
                }
            }
        }

        private static void ReorderAllPreviews()
        {
            LayoutDocumentPaneGroup paneGroup = (LayoutDocumentPaneGroup)((LayoutPanel)dockManager.Layout.Children.ElementAt(0)).Children[0];
            foreach (LayoutDocumentPane a in paneGroup.Children)
            {
                for (int i = a.Children.Count - 2; i >= 0; i--)
                {
                    var frame = a.Children[i].Content as Frame;
                    if (frame == null) break;
                    var prev = TaskbarManager.Instance.TabbedThumbnail.GetThumbnailPreview(frame);
                    var nextprev =
                        TaskbarManager.Instance.TabbedThumbnail.GetThumbnailPreview(
                            (UIElement) a.Children[i + 1].Content);
                    if (prev == null || nextprev == null) break;
                    TabbedThumbnailManager.SetTabOrder(prev,
                                                       TaskbarManager.Instance.TabbedThumbnail.GetThumbnailPreview(
                                                           (UIElement) a.Children[i + 1].Content));
                }
            }

            SetActivePreview((UIElement)dockManager.ActiveContent);
        }

        private static Vector SetThumbnailOffset(Frame frame)
        {
            //Calculate peekoffset for frame preview
            if (!frame.IsVisible) return new Vector(0, 0);   //if not visible it cannot get screen points
            LayoutDocumentPaneGroup paneGroup = (LayoutDocumentPaneGroup) ((LayoutPanel)dockManager.Layout.Children.ElementAt(0)).Children[0];
            LayoutContent layDoc = null;
            foreach (LayoutDocumentPane a in paneGroup.Children)
            {
                layDoc = a.Children.Where(x => x.Content == frame).SingleOrDefault();
                if (layDoc != null)
                {
                    layDoc = a.SelectedContent; //calculate offset for active frame. It would be the same in LayoutPane
                    break;
                }
            }
            Vector absolutePreviewPosition;
            if(layDoc != null && !layDoc.IsFloating)
            {
                var framePositionInWnd = ((Frame)layDoc.Content).PointToScreen(new Point(0, 0));
                var windowVisual = PresentationSource.FromVisual(ParentWindow);
                var windowLeftTop = windowVisual.CompositionTarget.RootVisual.PointToScreen(new Point(0, 0));
                absolutePreviewPosition = framePositionInWnd - windowLeftTop;
            }
            else
            {
                //Does not work
                absolutePreviewPosition = new Vector(Screen.PrimaryScreen.WorkingArea.Width/2 - frame.Width/2, Screen.PrimaryScreen.WorkingArea.Height/2 - frame.Height/2);
            }

            return new Vector(absolutePreviewPosition.X, absolutePreviewPosition.Y);
        }

        private static void RefreshPreview(Frame frame)
        {
            if (frame == null) return;
            SetThumbnailTitleAndIcon(frame, (IControlView) frame.Content);
            UpdateBitmap(frame);
        }

        //public for ThumbnailButtonsHelper to simply refresh previews
        public static void DelayedRefreshPreview(Frame frame)
        {
            previewCreatorDelay.Stop();
            previewCreatorDelay.Start();    
        }

        private static void RefreshAllPreviews()
        {
            LayoutDocumentPaneGroup paneGroup = (LayoutDocumentPaneGroup)((LayoutPanel)dockManager.Layout.Children.ElementAt(0)).Children[0];
            foreach (LayoutDocumentPane a in paneGroup.Children)
            {
                foreach (var frame in a.Children.Select(tab => tab.Content))
                {
                    RefreshPreview((Frame) frame);
                }
            }
        }

        private static void SetActivePreview(UIElement content)
        {
            if (content != null)
            {
                if (TaskbarManager.Instance.TabbedThumbnail.IsThumbnailPreviewAdded(content))
                {
                    TaskbarManager.Instance.TabbedThumbnail.SetActiveTab(content);
                }
            }
        }

        //bug fixing for closing tab from preview
        private static void CloseTab(Frame frame)
        {
            ((MainWindow)ParentWindow).CloseTab(frame);
        }

        private static void RemovePreview(UIElement content)
        {
            if (content != null)
            {
                if (TaskbarManager.Instance.TabbedThumbnail.IsThumbnailPreviewAdded(content))
                {
                    //Clean it
                    var preview = TaskbarManager.Instance.TabbedThumbnail.GetThumbnailPreview(content);
                    ((Frame) content).ContentRendered -= frame_ContentRendered;
                    ThumbnailButtonsHelper.RemoveButtons(preview);
                    TaskbarManager.Instance.TabbedThumbnail.RemoveThumbnailPreview(preview);
                    preview.TabbedThumbnailActivated -= thumbnail_TabbedThumbnailActivated;
                    preview.TabbedThumbnailClosed -= thumbnail_TabbedThumbnailClosed;
                    preview.TabbedThumbnailMaximized -= thumbnail_TabbedThumbnailMaximized;
                    preview.TabbedThumbnailMinimized -= thumbnail_TabbedThumbnailMinimized;
                    preview.Dispose();
                    preview = null;
                }
            }
        }
        
        //Taken from Windows API Code Pack, little changed
        //bug fix: TabbedThumbnailScreenCapture.GrabWindowBitmap sometimes tried to generane bitmap with infinite content bounds.
        //bug fix: GrabWindowBitmap's bounds checking sometimes was not enough. Couldn't generate preview.
        /// <summary>
        /// Grabs a snapshot of a WPF UIElement and returns the image as Bitmap.
        /// </summary>
        /// <param name="element">Represents the element to take the snapshot from.</param>
        /// <param name="dpiX">Represents the X DPI value used to capture this snapshot.</param>
        /// <param name="dpiY">Represents the Y DPI value used to capture this snapshot.</param>
        /// <param name="width">The requested bitmap width.</param>
        /// <param name="height">The requested bitmap height.</param>
        /// <returns>Returns the bitmap (PNG format).</returns>
        private static Bitmap GrabWindowBitmap(UIElement element, int dpiX, int dpiY, int width, int height)
        {
            Rect bounds = VisualTreeHelper.GetDescendantBounds(element);
            //bounds checked manualy before calling this method

            if (bounds.Height == 0 || bounds.Width == 0 || bounds.IsEmpty)
                return null;    // 0 sized element or infinite sized element. Probably hidden or not rendered yet.

            // create the renderer.
            RenderTargetBitmap rendertarget = new RenderTargetBitmap((int)(bounds.Width * dpiX / 96.0),
             (int)(bounds.Height * dpiY / 96.0), dpiX, dpiY, PixelFormats.Default);

            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext ctx = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(element);
                ctx.DrawRectangle(vb, null, new Rect(new System.Windows.Point(), bounds.Size));
            }

            rendertarget.Render(dv);

            BitmapEncoder bmpe;

            bmpe = new PngBitmapEncoder();
            bmpe.Frames.Add(BitmapFrame.Create(rendertarget));

            // Create a MemoryStream with the image.
            // Returning this as a MemoryStream makes it easier to save the image to a file or simply display it anywhere.
            MemoryStream fl = new MemoryStream();
            bmpe.Save(fl);

            Bitmap bmp = new Bitmap(fl);

            fl.Close();

            return (Bitmap)bmp.GetThumbnailImage(width, height, null, IntPtr.Zero);
        }

        private static void UpdateBitmap(ContentControl content)
        {
            var frame = content as Frame;
            if (frame == null) return;
            TabbedThumbnail preview = TaskbarManager.Instance.TabbedThumbnail.GetThumbnailPreview(frame);
            if (preview == null) return;
            //Check for offset changes
            preview.PeekOffset = SetThumbnailOffset(frame);
            //zak³adamy, ¿e wszystkie podgl¹dy s¹ wielkoœci tego aktywnego
            var activeContent = (Frame) dockManager.ActiveContent;
            LayoutDocumentPaneGroup paneGroup = (LayoutDocumentPaneGroup)((LayoutPanel)dockManager.Layout.Children.ElementAt(0)).Children[0];
            LayoutContent layDoc = null;
            foreach (LayoutDocumentPane a in paneGroup.Children)
            {
                layDoc = a.Children.Where(x => x.Content == frame).SingleOrDefault();
                if (layDoc != null)
                {
                    break;
                }
            }
            int previewWidth;
            int previewHeight;
            if(layDoc != null && !layDoc.IsFloating)
            {
                previewWidth = (int)(activeContent.ActualWidth == 0 ? frame.ActualWidth : activeContent.ActualWidth);
                previewHeight = (int)(activeContent.ActualHeight == 0 ? frame.ActualHeight : activeContent.ActualHeight);
            }
            else
            {
                previewWidth = 400;
                previewHeight = 300;
            }
            //podgl¹d nie mo¿e byæ wiêkszy ni¿ aktualne okno -> odœwie¿amy frame'a
            if(ParentWindow.ActualWidth < previewWidth || ParentWindow.ActualHeight < previewHeight)
            {
                frame.UpdateLayout();
                return;
            }

            Bitmap bitmap = GrabWindowBitmap((UIElement)frame.Content, 96, 96, previewWidth, previewHeight);

            if (bitmap != null)
            {
                preview.SetImage(bitmap);
                bitmap.Dispose();
                bitmap = null;
            }
            else  //Content isn't rendered yet, create preview from it's icon
            {
                var activeView = frame.Content as IControlView;
                var streamResourceInfo = Application.GetResourceStream(activeView.HeaderIcon);
                Bitmap rect = new Bitmap(previewWidth, previewHeight);
                if (streamResourceInfo != null)
                {
                    Stream iconStream = streamResourceInfo.Stream;
                    bitmap = new Bitmap(iconStream);
                    Graphics g = Graphics.FromImage(rect);
                    var bgColor = (Color)Application.Current.Resources["ControlBackgroundColor"];
                    g.FillRectangle(new SolidBrush(bgColor.ToDrawingColor()), 0, 0, previewWidth, previewHeight);
                    g.DrawImage(bitmap,(previewWidth/2)-(bitmap.Width/2), (previewHeight/2)-(bitmap.Height/2), 32, 32);
                    g.Save();
                    g.Dispose();
                }
                if (bitmap != null)
                {
                    preview.SetImage(rect);
                    bitmap.Dispose();
                    rect.Dispose();
                    bitmap = null;
                }
            }
        }

        #endregion

        #region Event handlers

        private static void ParentWindow_RibbonStateChange(object sender, EventArgs e)
        {
            if (dockManager.ActiveContent == null) return;
            resizeTimer.Stop();
            resizeTimer.Start();
        }

        private static void ResizingDone(object sender, EventArgs eventArgs)
        {
            resizeTimer.Stop();
            //_previewOffset = SetThumbnailOffset((Frame)dockManager.ActiveContent);
            RefreshAllPreviews();

        }

        private static void WaitingForRefreshDone(object sender, EventArgs eventArgs)
        {
            previewCreatorDelay.Stop();
            RefreshPreview((Frame) dockManager.ActiveContent);
        }
        
        private static void MovingDone(object sender, EventArgs eventArgs)
        {
            moveTimer.Stop();
            ReorderAllPreviews();
        }

        private static void ParentWindow_SizeStateChanged(object sender, EventArgs e)
        {
            //Nie trzeba odrysowywaæ podgl¹du przy minimalizacji okna
            if (ParentWindow.WindowState == WindowState.Minimized) return; 
            resizeTimer.Stop();
            resizeTimer.Start();
        }

        private static void thumbnail_TabbedThumbnailMinimized(object sender, TabbedThumbnailEventArgs e)
        {
            ParentWindow.WindowState = WindowState.Minimized;
        }

        private static void thumbnail_TabbedThumbnailMaximized(object sender, TabbedThumbnailEventArgs e)
        {
            ParentWindow.WindowState = WindowState.Maximized;
            thumbnail_TabbedThumbnailActivated(sender, e);
        }
        
        private static void thumbnail_TabbedThumbnailClosed(object sender, TabbedThumbnailClosedEventArgs e)
        {
            //var layoutDoc = LayoutPane.Children.Where(x => x.Content == e.WindowsControl).SingleOrDefault();
            LayoutDocumentPaneGroup paneGroup = (LayoutDocumentPaneGroup) ((LayoutPanel)dockManager.Layout.Children.ElementAt(0)).Children[0];
            LayoutContent layoutDoc = null;
            foreach (LayoutDocumentPane a in paneGroup.Children)
            {
                layoutDoc = a.Children.Where(x => x.Content == e.WindowsControl).SingleOrDefault();
                if (layoutDoc != null)
                {
                    break;
                }
            }
            if (layoutDoc == null) return;
            var controlView = layoutDoc.Content as IControlView;
            if (controlView == null) controlView = ((Frame) layoutDoc.Content).Content as IControlView;
            if(controlView != null && controlView.IsModified)
            {
               BAMessageBox.ShowInfo(Strings.Message_MainWindow_ShowPage_ModifiedView);
            }
            layoutDoc.ContentId = "CodeRemoval";
            layoutDoc.Parent.RemoveChild(layoutDoc);
            SetActivePreview((UIElement)dockManager.ActiveContent);
        }

        private static void thumbnail_TabbedThumbnailActivated(object sender, TabbedThumbnailEventArgs e)
        {
            Frame content = e.WindowsControl as Frame;
            if (content != null)
            {
                //var currentIndex = LayoutPane.Children.IndexOf();
                int currentIndex;
                LayoutDocumentPaneGroup paneGroup = (LayoutDocumentPaneGroup) ((LayoutPanel)dockManager.Layout.Children.ElementAt(0)).Children[0];
                foreach (LayoutDocumentPane a in paneGroup.Children)
                {
                    var layDoc = a.Children.Where(x => x.Content == content).SingleOrDefault();
                    if (layDoc != null)
                    {
                        currentIndex = a.Children.IndexOf(layDoc);
                        a.SelectedContentIndex = currentIndex;
                        dockManager.ActiveContent = content;
                        if (ParentWindow.WindowState == WindowState.Minimized)
                            ParentWindow.WindowState = WindowState.Maximized;
                        if (!ParentWindow.IsActive)
                            ParentWindow.Activate();
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Find LayoutDocumentPane with <paramref name="frame"/> inside
        /// </summary>
        private static LayoutDocumentPane FindLayoutDocumentPane(Frame frame)
        {
            LayoutDocumentPaneGroup paneGroup =
                (LayoutDocumentPaneGroup) ((LayoutPanel) dockManager.Layout.Children.ElementAt(0)).Children[0];
            return (from LayoutDocumentPane a in paneGroup.Children let layDoc = a.Children.Where(x => x.Content == frame).SingleOrDefault() where layDoc != null select a).FirstOrDefault();
        }

        private static void dockManager_ActiveContentChanged(object sender, EventArgs e)
        {
            if (dockManager.ActiveContent == null) return; 
            var content = dockManager.ActiveContent as UIElement;
            SetActivePreview(content);
            //Delay, because we need to wait a lil bit for new content to render
            DelayedRefreshPreview((Frame) content);    
        }

        
        static void Layout_ElementRemoved(object sender, LayoutElementEventArgs e)
        {
            LayoutDocument layoutDoc = e.Element as LayoutDocument;
            var floating = e.Element as LayoutDocumentFloatingWindow;
            var layoutPanel = e.Element as LayoutDocumentPane;
            if (layoutDoc != null)
            {
                var frame = layoutDoc.Content as Frame;
                if (layoutDoc.ContentId == "CodeRemoval")
                {
                    if (TaskbarManager.Instance.TabbedThumbnail.IsThumbnailPreviewAdded(frame))
                        ThumbnailButtonsHelper.RemoveButtons(TaskbarManager.Instance.TabbedThumbnail.GetThumbnailPreview(frame));
                    //Tab closed from preview -> clean frame handlers
                    frame.ContentRendered -= frame_ContentRendered;
                    CloseTab(frame);
                    return;
                }
                if (frame != null)
                {
                    RemovePreview(frame);
                }

            }
            else if (floating != null)
            {
                return;
            }
            else if (layoutPanel != null)
            {
                return;
            }
        }

        static void Layout_ElementAdded(object sender, LayoutElementEventArgs e)
        {
            LayoutDocument layoutDoc = e.Element as LayoutDocument;
            var floating = e.Element as LayoutDocumentFloatingWindow;
            var layoutPanel = e.Element as LayoutDocumentPane;
            if (layoutDoc != null)
            {
                Frame frame = layoutDoc.Content as Frame;
                if (frame != null)
                {
                    //Add new frame rendered event handler. Create frame preview after it is rendered
                    frame.ContentRendered -= frame_ContentRendered;
                    frame.ContentRendered += frame_ContentRendered;
                    //refreshing frame to force calling frame_ContentRendered after adding new tab from flowing
                    frame.Refresh();
                }
                
            }
            else if(floating != null)
            {
                layoutDoc = floating.RootDocument as LayoutDocument;
                if (layoutDoc != null)
                {
                    Frame frame = layoutDoc.Content as Frame;
                    if (frame != null)
                    {
                        //Add new frame rendered event handler. Create frame preview after it is rendered
                        frame.ContentRendered -= frame_ContentRendered;
                        frame.ContentRendered += frame_ContentRendered;
                        //refreshing frame to force calling frame_ContentRendered after adding new tab from flowing
                        frame.Refresh();
                    }
                }
            }
            else if(layoutPanel != null)
            {
                layoutDoc = layoutPanel.SelectedContent as LayoutDocument;
                if (layoutDoc != null)
                {
                    Frame frame = layoutDoc.Content as Frame;
                    if (frame != null)
                    {
                        //Add new frame rendered event handler. Create frame preview after it is rendered
                        frame.ContentRendered -= frame_ContentRendered;
                        frame.ContentRendered += frame_ContentRendered;
                        //refreshing frame to force calling frame_ContentRendered after adding new tab from flowing
                        frame.Refresh();
                    }
                }
            }
            RefreshAllPreviews();
        }



        private static void frame_ContentRendered(object sender, EventArgs e)
        {
            var frame = (Frame) sender;
            //Create new preview if it doesn't exist yet (check inside CreateContentPreview)
            CreateContentPreview(frame);
            SetThumbnailTitleAndIcon(frame, (IControlView)frame.Content);
            ReorderAllPreviews();
            DelayedRefreshPreview(frame);
        }

        #endregion
    }
}
