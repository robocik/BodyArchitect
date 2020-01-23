using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.WindowsAPICodePack.Taskbar;

namespace BodyArchitect.Client.UI.WindowsSevenIntegration
{
    public static class ThumbnailButtonsHelper
    {
        private static Dictionary<TabbedThumbnail, List<ThumbnailToolBarButton>> buttonList = new Dictionary
            <TabbedThumbnail, List<ThumbnailToolBarButton>>();

        //TODO:translate
        private const string forwardTooltip = "Forward";
        private const string backwardTooltip = "Backward";

        public static void AddTaskbarButtons(UIElement content)
        {
            ThumbnailToolBarButton refresh = AddRefreshBtn();
            ThumbnailToolBarButton forward = AddForwardBtn();
            ThumbnailToolBarButton back = AddBackBtn();
            if (content == null || refresh == null || forward == null || back == null) return;
            try
            {
                TaskbarManager.Instance.ThumbnailToolBars.AddButtons(content, back, refresh, forward);
                var tempList = new List<ThumbnailToolBarButton>();
                tempList.Add(back);
                tempList.Add(refresh);
                tempList.Add(forward);
                var preview = TaskbarManager.Instance.TabbedThumbnail.GetThumbnailPreview(content);
                buttonList.Add(preview, tempList);
            } 
            catch (Exception e)
            {
                Console.Error.WriteLine(e.ToString());
            }
        }

        public static void RemoveButtons(TabbedThumbnail preview)
        {
            if (buttonList.ContainsKey(preview))
            {
                buttonList.Remove(preview);
            }
        }

        public static void UpdateButtonStatus(Frame frame)
        {
            var preview = TaskbarManager.Instance.TabbedThumbnail.GetThumbnailPreview(frame);
            if (preview == null) return;
            if(buttonList.ContainsKey(preview))
            {
                var btns = new List<ThumbnailToolBarButton>();
                buttonList.TryGetValue(preview, out btns);
                foreach (var btn in btns)
                {
                    switch(btn.Tooltip)
                    {
                        case forwardTooltip:
                            btn.Enabled = frame.CanGoForward;
                            break;
                        case backwardTooltip:
                            btn.Enabled = frame.CanGoBack;
                            break;
                    }
                }
            }
        }

        private static ThumbnailToolBarButton AddRefreshBtn()
        {
            ThumbnailToolBarButton refresh = null;
            var streamResourceInfo = Application.GetResourceStream(new Uri("/BodyArchitect.Client.Resources;component/Images/Refresh16.png", UriKind.Relative));
            if (streamResourceInfo != null)
            {
                Stream iconStream = streamResourceInfo.Stream;
                var bmp = new Bitmap(iconStream);
                var icon = Icon.FromHandle(bmp.GetHicon());
                //TODO:Translate
                refresh = new ThumbnailToolBarButton(icon, "Refresh");
                refresh.Click += (sender, e) =>
                                      {
                                          var content = e.WindowsControl as Frame;
                                          if (content != null)
                                          {
                                              var control = content.Content as IControlView;
                                              if (control != null)
                                              {
                                                  control.RefreshView();
                                                  TThumbnailHelper.DelayedRefreshPreview(content);
                                              }
                                          }
                                      };
            }
            return refresh;
        }

        private static ThumbnailToolBarButton AddForwardBtn()
        {
            ThumbnailToolBarButton forward = null;
            var streamResourceInfo = Application.GetResourceStream(new Uri("/BodyArchitect.Client.Resources;component/Images/Forward16.png", UriKind.Relative));
            if (streamResourceInfo != null)
            {
                Stream iconStream = streamResourceInfo.Stream;
                var bmp = new Bitmap(iconStream);
                var icon = Icon.FromHandle(bmp.GetHicon());
                forward = new ThumbnailToolBarButton(icon, forwardTooltip);
                forward.Click += (sender, e) =>
                                     {
                                         var content = e.WindowsControl as Frame;
                                         if (content != null && content.CanGoForward)
                                         {
                                             content.GoForward();
                                         }
                                     };
            }
            return forward;
        }

        private static ThumbnailToolBarButton AddBackBtn()
        {
            ThumbnailToolBarButton back = null;
            var streamResourceInfo = Application.GetResourceStream(new Uri("/BodyArchitect.Client.Resources;component/Images/Back16.png", UriKind.Relative));
            if (streamResourceInfo != null)
            {
                Stream iconStream = streamResourceInfo.Stream;
                var bmp = new Bitmap(iconStream);
                var icon = Icon.FromHandle(bmp.GetHicon());
                back = new ThumbnailToolBarButton(icon, backwardTooltip);
                back.Click += (sender, e) =>
                                  {
                                      var content = e.WindowsControl as Frame;
                                      if (content != null && content.CanGoBack)
                                      {
                                          content.GoBack();
                                      }
                                  };
            }
            return back;
        }
    }
}
