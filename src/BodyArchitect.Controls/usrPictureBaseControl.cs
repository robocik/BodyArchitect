
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using BodyArchitect.Common;
using BodyArchitect.Controls.Cache;
using BodyArchitect.Service.Model;

namespace BodyArchitect.Controls
{
    public class usrPictureBaseControl:usrBaseControl
    {
        private IHasPicture pictureOwner;
        private PictureBox pictureBox;

        public usrPictureBaseControl()
        {
            if (!ServicesManager.IsDesignMode)
            {
                PicturesCache.Instance.PictureLoaded += new EventHandler<PictureInfoEventArgs>(Instance_PictureLoaded);
            }
        }

        public void Fill(PictureBox pictureBox, IHasPicture pictureOwner)
        {
            this.pictureBox = pictureBox;
            this.pictureOwner = pictureOwner;

            if (pictureOwner==null)
            {
                pictureBox.Image = null;
                return;
            }
            if (pictureOwner.Picture == null)
            {
                pictureBox.Image = Icons.DefaultProfile;
            }
            else
            {
                var cacheItem = PicturesCache.Instance.GetImage(pictureOwner.Picture);
                if (cacheItem == null)
                {
                    pictureBox.Image = Icons.DefaultProfile;
                }
                else
                {
                    if (cacheItem.Image == null)
                    {
                        pictureBox.Image = pictureBox.ErrorImage;
                    }
                    else
                    {
                        pictureBox.Image = cacheItem.Image;
                    }
                }
            }
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (!ServicesManager.IsDesignMode)
            {
                PicturesCache.Instance.PictureLoaded -= new EventHandler<PictureInfoEventArgs>(Instance_PictureLoaded);
            }
            base.OnHandleDestroyed(e);
        }

        void Instance_PictureLoaded(object sender, PictureInfoEventArgs e)
        {
            if (pictureOwner != null && pictureOwner.Picture != null && pictureOwner.Picture.PictureId == e.PictureInfo.PictureId)
            {
                var cacheItem = PicturesCache.Instance.GetImage(e.PictureInfo);
                pictureOwner.Picture.Hash = cacheItem.Hash;
                setImage(cacheItem.Image);
            }
        }

        private void setImage(Image image)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Image>(setImage), image);
            }
            else
            {
                pictureBox.Image = image;
            }
            pictureBox.Image = image;
        }
    }
}
