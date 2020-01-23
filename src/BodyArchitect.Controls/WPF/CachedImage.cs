using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using BodyArchitect.Controls.Cache;
using BodyArchitect.Service.Model;
using Image = System.Windows.Controls.Image;

namespace BodyArchitect.Controls.WPF
{
    public class CachedImage:Image
    {
        private static BitmapSource defaultProfile;

        public static BitmapSource DefaultProfile
        {
            get
            {
                if(defaultProfile==null)
                {
                    defaultProfile =  GetBitmapSource(Icons.DefaultProfile);
                }
                return defaultProfile;
            }
        }

        public CachedImage()
        {
            Loaded += new System.Windows.RoutedEventHandler(CachedImage_Loaded);
            Unloaded += new System.Windows.RoutedEventHandler(CachedImage_Unloaded);
        }

        public UserDTO PictureOwner
        {

            //get { return (UserDTO)GetValue(PictureOwnerProperty); }
            get
            {
                return (UserDTO)this.Dispatcher.Invoke(
                   System.Windows.Threading.DispatcherPriority.Background,
                   (DispatcherOperationCallback)delegate { return GetValue(PictureOwnerProperty); },PictureOwnerProperty);
            }
            set { SetValue(PictureOwnerProperty, value); }
        }


        public static readonly DependencyProperty PictureOwnerProperty =
                DependencyProperty.Register("PictureOwner",
                typeof(UserDTO),typeof(CachedImage),
                new PropertyMetadata(null, OnColorChanged));

        public void Fill( IHasPicture pictureOwner)
        {

            if (pictureOwner == null)
            {
                Source = null;
                return;
            }
            if (pictureOwner.Picture == null)
            {
                Source = DefaultProfile;
            }
            else
            {
                var cacheItem = PicturesCache.Instance.GetImage(pictureOwner.Picture);
                if (cacheItem == null)
                {
                    Source = DefaultProfile;
                }
                else
                {
                    if (cacheItem.Image == null)
                    {
                        Source = DefaultProfile;
                    }
                    else
                    {
                        Source = GetBitmapSource((Bitmap)cacheItem.Image);
                    }
                }
            }
        }

        static void OnColorChanged(DependencyObject obj,DependencyPropertyChangedEventArgs args)
        {
            CachedImage img = (CachedImage) obj;
            img.Fill((UserDTO)args.NewValue);
            //var usr =(PictureInfoEventArgs) args.NewValue;
        }

        void CachedImage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            PicturesCache.Instance.PictureLoaded += new EventHandler<PictureInfoEventArgs>(Instance_PictureLoaded);
        }

        void CachedImage_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            PicturesCache.Instance.PictureLoaded -= new EventHandler<PictureInfoEventArgs>(Instance_PictureLoaded);
        }

        void Instance_PictureLoaded(object sender, PictureInfoEventArgs e)
        {
            if (PictureOwner != null && PictureOwner.Picture != null && PictureOwner.Picture.PictureId == e.PictureInfo.PictureId)
            {
                var cacheItem = PicturesCache.Instance.GetImage(e.PictureInfo);
                PictureOwner.Picture.Hash = cacheItem.Hash;
                setImage(cacheItem.Image);
            }
        }

        private void setImage(System.Drawing.Image image)
        {
            Dispatcher.BeginInvoke(new Action(delegate
                                                  {
                                                      if (image != null)
                                                      {
                                                          Source = GetBitmapSource((Bitmap) image);
                                                      }
                                                      else
                                                      {
                                                          Source = DefaultProfile;
                                                      }
                                                  }));

        }

        private static System.Windows.Media.Imaging.BitmapSource GetBitmapSource(System.Drawing.Bitmap _image)
        {
            System.Drawing.Bitmap bitmap = _image;
            System.Windows.Media.Imaging.BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    bitmap.GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            return bitmapSource;
        }
    }
}
