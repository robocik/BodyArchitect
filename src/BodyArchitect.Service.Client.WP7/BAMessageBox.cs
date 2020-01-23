using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Clarity.Phone.Extensions;
using Coding4Fun.Phone.Controls;

namespace BodyArchitect.Service.Client.WP7
{
    public static class BAMessageBox
    {
        public static void ShowError(string message, bool forceMessagePrompt = false)
        {
            showMessageImplementation(message, Colors.Red, forceMessagePrompt);
        }

        public static void ShowWarning(string message, bool forceMessagePrompt = false)
        {
            showMessageImplementation(message, Colors.Orange, forceMessagePrompt);
        }

        static void showMessageImplementation(string message, Color backColor, bool forceMessagePrompt, Uri image = null)
        {
            if(Deployment.Current.CheckAccess())
            {
                showMsg(message, backColor, forceMessagePrompt, image);
            }
            else
            {
                Deployment.Current.Dispatcher.BeginInvoke(() => showMsg(message, backColor, forceMessagePrompt, image));
            }

            
        }

        private static void showMsg(string message, Color backColor, bool forceMessagePrompt,Uri image=null)
        {
//fix for crash after tombstoning for ToastPrompt
            var obj = Application.Current.RootVisual.GetVisualDescendants().OfType<ContentPresenter>();
            if (obj.Count() == 0)
            {
                return;
            }
            //show toast only when message is quite short
            if (forceMessagePrompt || message.Length >= 80)
            {
                MessagePrompt t = new MessagePrompt();
                t.IsAppBarVisible = true; //fix for crash when MessagePrompt is showed in OnNavigatedTo method
                var text = new TextBlock { Text = message, TextWrapping = TextWrapping.Wrap };
                text.Foreground = (Brush)Application.Current.Resources["CustomContrastForegroundBrush"];
                Grid grid= new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition(){Width = GridLength.Auto});
                grid.ColumnDefinitions.Add(new ColumnDefinition());

                if (image != null)
                {
                    Image img = new Image();
                    img.Source = new BitmapImage(image);
                    Grid.SetColumn(img,0);
                }
                Grid.SetColumn(text, 1);
                t.Body = text;
                t.Background = new SolidColorBrush(backColor);
                
                t.Show();
            }
            else
            {
                ToastPrompt t = new ToastPrompt();
                t.Message = message;
                t.IsAppBarVisible = true;
                t.Foreground = (Brush) Application.Current.Resources["CustomContrastForegroundBrush"];
                t.Background = new SolidColorBrush(backColor);
                if (image != null)
                {
                    t.ImageSource = new BitmapImage(image);
                }
                t.Show();
            }
        }

        public static MessageBoxResult Ask(string message)
        {
            //MessagePrompt t = new MessagePrompt();
            //t.Body = message;
            //t.IsCancelVisible = true;
            //t.Show();
            //return MessageBoxResult.OK;
            //try catch is for strange error (0x8000ffff) 
            try
            {
                return MessageBox.Show(message, "BodyArchitect", MessageBoxButton.OKCancel);
            }
            catch (Exception)
            {
                return MessageBoxResult.Cancel;
            }
            
        }


        public static void ShowInfo(string message,Uri image=null, bool forceMessagePrompt = false)
        {
            showMessageImplementation(message, Color.FromArgb(255, 159, 215, 245), forceMessagePrompt, image);
        }
    }
}
