using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.Shared;
using BodyArchitect.WCF;
using BodyArchitect.Common;
using BodyArchitect.Controls.Cache;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Logger;
using BodyArchitect.Service.Model;
using DevExpress.XtraEditors;

namespace BodyArchitect.Controls.Basic
{
    public class BaPictureEdit:PictureEdit
    {
        private PictureInfoDTO pictureInfo;
        private bool pictureLoaded;

        public void Fill(ProfileDTO profile)
        {
            if(pictureLoaded)
            {
                return;
            }
            Image image = null;
            if (profile.Picture != null)
            {
                try
                {
                    profile.Picture.SessionId = UserContext.Token.SessionId;
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
                    setText( ApplicationStrings.PhotoDoesntExist);
                }
                
            }
            setControls(image);
            pictureLoaded = true;
        }

        void setControls(Image image)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action< Image>(setControls), image);
            }
            else
            {
                Image = image;
            }
        }

        void setText(string text)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(setText), text);
            }
            else
            {
                Properties.NullText = text;
            }
        }

        protected override void OnDoubleClick(EventArgs e)
        {
            base.OnDoubleClick(e);
            LoadImage();
        }

        protected override void OnKeyUp(System.Windows.Forms.KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (e.KeyCode == Keys.Delete)
            {
                Image = null;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ErrorOccured { get; private set; }

        public PictureDTO Save(ProfileDTO profile)
        {
            if(!pictureLoaded)
            {
                return null;
            }
            PictureDTO pic = null;
            if (Image != null)
            {
                var stream = new MemoryStream();
                Image.Save(stream, ImageFormat.Png);
                stream.Seek(0, SeekOrigin.Begin);

                pic = new PictureDTO(UserContext.Token);

                pic.ImageStream = new MemoryStream();
                var resizedImage = Common.Helper.ResizeImage(Image, UserDTO.PictureMaxWidth, UserDTO.PictureMaxHeight, true);
                resizedImage.Save(pic.ImageStream, ImageFormat.Bmp);
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
            return pic;
        }
    }
}
