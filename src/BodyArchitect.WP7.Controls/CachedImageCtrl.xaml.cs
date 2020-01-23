using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls.Cache;

namespace BodyArchitect.WP7.Controls
{
    public partial class CachedImageCtrl : UserControl
    {
        public CachedImageCtrl()
        {
            InitializeComponent();
            Loaded += new System.Windows.RoutedEventHandler(CachedImage_Loaded);
            Unloaded += new System.Windows.RoutedEventHandler(CachedImage_Unloaded);
        }

        private static BitmapSource defaultProfile;
        private PictureInfoDTO picture;

        public static BitmapSource DefaultProfile
        {
            get
            {
                if (defaultProfile == null)
                {
                    defaultProfile = new BitmapImage(new Uri("/Images/defaultProfile.png",UriKind.Relative));
                }
                return defaultProfile;
            }
        }



        public void Fill(PictureInfoDTO picture)
        {
            this.picture = picture;
            if (picture == null || picture == PictureInfoDTO.Empty)
            {
                imgImage.Source = DefaultProfile;
                return;
            }

            else
            {
                var cacheItem = PicturesCache.Instance.GetImage(picture);
                if (cacheItem == null)
                {
                    imgImage.Source = DefaultProfile;
                }
                else
                {
                    if (cacheItem.Image == null)
                    {
                        imgImage.Source = DefaultProfile;
                    }
                    else
                    {
                        imgImage.Source = cacheItem.Image;
                    }
                }
            }
        }

        void CachedImage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            PicturesCache.Instance.PictureLoaded += new EventHandler<PictureInfoEventArgs>(Instance_PictureLoaded);
        }

        void CachedImage_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            PicturesCache.Instance.PictureLoaded -= new EventHandler<PictureInfoEventArgs>(Instance_PictureLoaded);
        }

        static void OnColorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            CachedImageCtrl img = (CachedImageCtrl)obj;
            img.Fill((PictureInfoDTO)args.NewValue);
        }

        static void OnSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            CachedImageCtrl img = (CachedImageCtrl)obj;
            img.imgImage.Source=(BitmapSource)args.NewValue;
        }
        public PictureInfoDTO Picture
        {
            get
            {
                return (PictureInfoDTO)GetValue(PictureProperty);
            }
            set { SetValue(PictureProperty, value); }
        }

        public BitmapSource Source
        {
            get
            {
                return (BitmapSource)GetValue(SourceProperty);
            }
            set { SetValue(SourceProperty, value); }
        }

        public static readonly DependencyProperty PictureProperty =
                DependencyProperty.Register("Picture",
                typeof(PictureInfoDTO), typeof(CachedImageCtrl),
                new PropertyMetadata(null, OnColorChanged));

        public static readonly DependencyProperty SourceProperty =
                DependencyProperty.Register("Source",
                typeof(BitmapSource), typeof(CachedImageCtrl),
                new PropertyMetadata(null, OnSourceChanged));

        void Instance_PictureLoaded(object sender, PictureInfoEventArgs e)
        {
            if (picture != null && picture.PictureIdk__BackingField == e.PictureInfo.PictureIdk__BackingField)
            {
                var cacheItem = PicturesCache.Instance.GetImage(e.PictureInfo);
                picture.Hashk__BackingField = cacheItem.Hash;
                if (cacheItem.Image != null)
                {
                    if (Dispatcher.CheckAccess())
                    {
                        imgImage.Source = cacheItem.Image;
                    }
                    else
                    {
                        Dispatcher.BeginInvoke(delegate
                        {
                            imgImage.Source = cacheItem.Image;
                        });
                    }
                }
                else
                {
                    Dispatcher.BeginInvoke(delegate
                                               {
                                                   imgImage.Source = new BitmapImage(new Uri("/Images/defaultProfile.png", UriKind.Relative));
                                               });
                    
                }
            }
        }
    }
}
