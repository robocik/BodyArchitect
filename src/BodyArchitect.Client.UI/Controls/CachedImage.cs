using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.UI.Cache;
using BodyArchitect.Service.V2.Model;
using Image = System.Windows.Controls.Image;

namespace BodyArchitect.Client.UI.Controls
{
    public class CachedImage : Image
    {
        private bool defaultImageLoaded = true;
        private static BitmapSource defaultProfile;

        public static BitmapSource DefaultProfile
        {
            get
            {
                if (defaultProfile == null)
                {
                    defaultProfile = "pack://application:,,,/BodyArchitect.Client.Resources;component/Images/DefaultProfile.png".ToBitmap();
                }
                return defaultProfile;
            }
        }

        public BitmapSource DefaultImage
        {
            get { return (BitmapSource)GetValue(DefaultImageProperty); }
            set
            {
                SetValue(DefaultImageProperty, value);
            }
        }


        public static readonly DependencyProperty DefaultImageProperty =
            DependencyProperty.Register("DefaultImage", typeof(BitmapSource), typeof(CachedImage), new UIPropertyMetadata(DefaultProfile, OnDefaultImageChanged));



        private static void OnDefaultImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            var ctrl = (CachedImage)d;

            if (ctrl.defaultImageLoaded )
            {
                ctrl.Source = (ImageSource) e.NewValue;
            }


        }

        public CachedImage()
        {

            PicturesCache.Instance.PictureLoaded += new EventHandler<PictureInfoEventArgs>(Instance_PictureLoaded);
            Unloaded += new System.Windows.RoutedEventHandler(CachedImage_Unloaded);
        }

        public IHasPicture PictureOwner
        {
            get
            {
                if(Dispatcher.CheckAccess())
                {
                    return (IHasPicture)GetValue(PictureOwnerProperty);
                }
                else
                {
                    return (IHasPicture)this.Dispatcher.Invoke(DispatcherPriority.Background,(DispatcherOperationCallback)delegate { return GetValue(PictureOwnerProperty); }, PictureOwnerProperty);
                }
                
            }
            set { SetValue(PictureOwnerProperty, value); }
        }


        public static readonly DependencyProperty PictureOwnerProperty =
                DependencyProperty.Register("PictureOwner",
                typeof(IHasPicture), typeof(CachedImage),
                new PropertyMetadata(null, OnColorChanged));

        void setDefaultImage()
        {
            defaultImageLoaded = true;
            Source = DefaultImage;
        }
        public void Fill(IHasPicture pictureOwner)
        {
            if (pictureOwner == null)
            {
                Source = null;
                defaultImageLoaded = false;
                return;
            }
            if (pictureOwner.Picture == null)
            {
                setDefaultImage();
            }
            else
            {
                var cacheItem = PicturesCache.Instance.GetImage(pictureOwner.Picture);
                if (cacheItem == null)
                {
                    setDefaultImage();
                }
                else
                {
                    setImage(cacheItem.Image);
                }
            }
        }

        static void OnColorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            CachedImage img = (CachedImage)obj;
            img.Fill((IHasPicture)args.NewValue);
        }

        void CachedImage_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            PicturesCache.Instance.PictureLoaded -= new EventHandler<PictureInfoEventArgs>(Instance_PictureLoaded);
        }

        void Instance_PictureLoaded(object sender, PictureInfoEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(delegate
            {
                if (PictureOwner != null && PictureOwner.Picture != null && PictureOwner.Picture.PictureId == e.PictureInfo.PictureId)
                {
                    var cacheItem = PicturesCache.Instance.GetImage(e.PictureInfo);
                    PictureOwner.Picture.Hash = cacheItem.Hash;
                    setImage(cacheItem.Image);
                }
            }));
            
        }

        private void setImage(System.Drawing.Image image)
        {
            if (image != null)
            {
                Source = image.GetBitmapSource();
                defaultImageLoaded = false;
            }
            else
            {
                setDefaultImage();
            }
        }
    }
}
