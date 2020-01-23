using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Client.UI.Cache;
using BodyArchitect.Client.UI.UserControls;
using BodyArchitect.Client.WCF;
using BodyArchitect.Logger;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using Microsoft.Win32;

namespace BodyArchitect.Client.UI.Controls
{
    /// <summary>
    /// Interaction logic for ExBaImage.xaml
    /// </summary>
    public partial class ExBaImage
    {
        private PictureInfoDTO pictureInfo;
        private bool pictureLoaded;
        private bool imageChanged;

        public ExBaImage()
        {
            InitializeComponent();
            setImage(null);
        }

        void setImage(ImageSource image)
        {
            if(image==null)
            {
                imgControl.Source = CachedImage.DefaultProfile;
                btnDelete.Visibility = Visibility.Collapsed;
            }
            else
            {
                imgControl.Source = image;
                btnDelete.SetVisible(!ReadOnly);
            }
        }

        public bool ReadOnly
        {
            get { return (bool)GetValue(ReadOnlyProperty); }
            set
            {
                SetValue(ReadOnlyProperty, value);
            }
        }


        public static readonly DependencyProperty ReadOnlyProperty =
            DependencyProperty.Register("ReadOnly", typeof(bool), typeof(ExBaImage), new UIPropertyMetadata(false));


        public void Fill(IHasPicture profile)
        {
            if (pictureLoaded)
            {
                return;
            }
            System.Drawing.Image image = null;
            if (profile!=null && profile.Picture != null)
            {
                try
                {
                    profile.Picture.SessionId = UserContext.Current.Token.SessionId;
                    var pictureDto = ServiceManager.GetImage(profile.Picture);
                    pictureInfo = pictureDto.GetPictureInfo();
                    var cacheItem = pictureDto.ToPictureCacheItem();
                    PicturesCache.Instance.AddToCache(cacheItem);
                    image = cacheItem.Image;
                    ErrorOccured = false;
                }
                catch (FileNotFoundException ex)
                {
                    ErrorOccured = true;
                    ExceptionHandler.Default.Process(ex);
                    setText(Strings.PhotoDoesntExist);
                }

            }
            setControls(image);
            pictureLoaded = true;
        }

        ImageSource createImageFromFIle(string filename)
        {
            MemoryStream stream = new MemoryStream(File.ReadAllBytes(filename));
            System.Windows.Media.Imaging.BitmapImage img = new System.Windows.Media.Imaging.BitmapImage();
            img.BeginInit();
            img.StreamSource = stream;
            img.EndInit();

            return img;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (!ReadOnly && e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Filter = Strings.ExBaImage_OpenImageDialogFilter;
                if (dlg.ShowDialog() == true)
                {
                    setImage(createImageFromFIle(dlg.FileName));
                    imageChanged = true;
                }
            }
            base.OnMouseDown(e);
        }
        void setControls(System.Drawing.Image image)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new Action<System.Drawing.Image>(setControls), image);
            }
            else
            {
                setImage(image!=null?image.GetBitmapSource():null);

            }
        }

        void setText(string text)
        {
            //if (InvokeRequired)
            //{
            //    BeginInvoke(new Action<string>(setText), text);
            //}
            //else
            //{
            //    Properties.NullText = text;
            //}
        }


        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ErrorOccured { get; private set; }

        public PictureDTO Save(IHasPicture profile)
        {
            if (!pictureLoaded)
            {
                return null;
            }
            PictureDTO pic = null;
            if (imageChanged)
            {
                if (btnDelete.Visibility == Visibility.Visible
                    /*gdy przycisk delete jest widoczny to znaczy ze jest wybrany niestandardowy obrazek*/)
                {
                    var stream = new MemoryStream();

                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create((BitmapSource) imgControl.Source));
                    encoder.Save(stream);
                    //Image.Save(stream, ImageFormat.Png);
                    stream.Seek(0, SeekOrigin.Begin);

                    pic = new PictureDTO(UserContext.Current.Token);

                    pic.ImageStream = new MemoryStream();
                    var resizedImage = Helper.Resize(stream, UserDTO.PictureMaxWidth, UserDTO.PictureMaxHeight,
                                                     BitmapScalingMode.HighQuality);

                    var targetEncoder = new PngBitmapEncoder();
                    targetEncoder.Frames.Add(resizedImage);
                    targetEncoder.Save(pic.ImageStream);
                    //targetBytes = memoryStream.ToArray();
                    //resizedImage.Save(pic.ImageStream, ImageFormat.Bmp);
                    pic.ImageStream.Seek(0, SeekOrigin.Begin);
                    pic.Hash = CryptographyHelper.GetMD5HashFromFile(pic.ImageStream);
                    pic.ImageStream.Seek(0, SeekOrigin.Begin);
                }
                else
                {
                    profile.Picture = null;
                    if (pictureInfo != null)
                    {
                        PicturesCache.Instance.Remove(pictureInfo.PictureId);
                    }
                }
            }
            return pic;
        }
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (!ReadOnly)
            {
                setImage(null);
                imageChanged = true;
            }
        }

        public void Clear()
        {
            setImage(null);
            imageChanged = false;
            pictureLoaded = false;
        }
    }
}
