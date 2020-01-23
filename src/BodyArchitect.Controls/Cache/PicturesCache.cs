using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using BodyArchitect.WCF;
using BodyArchitect.Service.Model;
using Microsoft.Practices.EnterpriseLibrary.Caching;
using Microsoft.Practices.EnterpriseLibrary.Caching.Expirations;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;


namespace BodyArchitect.Controls.Cache
{
    public class PictureInfoEventArgs:EventArgs
    {
        public PictureInfoEventArgs(PictureInfoDTO pictureInfo)
        {
            PictureInfo = pictureInfo;
        }

        public PictureInfoDTO PictureInfo { get; private set; }
    }

    public static class PictureCacheHelper
    {
        public static PictureCacheItem ToPictureCacheItem(this PictureDTO pictureDto)
        {
            if (pictureDto.ImageStream.CanSeek)
            {
                pictureDto.ImageStream.Seek(0, SeekOrigin.Begin);
            }
            MemoryStream stream = new MemoryStream();
            pictureDto.ImageStream.CopyTo(stream);
            stream.Seek(0, SeekOrigin.Begin);
            var image = Image.FromStream(stream);
            PictureCacheItem item = new PictureCacheItem(image,pictureDto.PictureId,pictureDto.Hash);
            return item;
        }
    }

    [Serializable]
    public class PictureCacheItem
    {
        public PictureCacheItem(Image image, Guid picureId, string hash)
        {
            Image = image;
            PictureId = picureId;
            Hash = hash;
        }

        public Image Image { get; private set; }
        public Guid PictureId { get; private set; }
        public string Hash { get; private set; }

        
    }

    public class PicturesCache
    {
        public const string PicturesCacheName = "PicturesCacheManager";
        public event EventHandler<PictureInfoEventArgs> PictureLoaded;
        private object syncObject=new object();
        static PicturesCache picturesCache;

        private PicturesCache()
        {
        }



        public static PicturesCache Instance
        {
            get
            {
                if(picturesCache==null)
                {
                    picturesCache=new PicturesCache();
                }
                return picturesCache;
            }
        }

        private ICacheManager cache;

        public ICacheManager Cache
        {
            get
            {
                lock (syncObject)
                {
                    if (cache == null)
                    {
                        try
                        {

                            cache = EnterpriseLibraryContainer.Current.GetInstance<ICacheManager>(PicturesCacheName);
                        }
                        catch (Exception)
                        {
                            cache = EnterpriseLibraryContainer.Current.GetInstance<ICacheManager>("ErrorCache");
                        }
                    }
                    return cache;
                }
                
            }
        }

        public void Remove(Guid pictureId)
        {
            Cache.Remove(pictureId.ToString());
        }

        public PictureCacheItem GetImage(PictureInfoDTO pictureInfo)
        {

            PictureCacheItem picture = (PictureCacheItem)Cache.GetData(pictureInfo.PictureId.ToString());

            if (picture == null || (picture.Image!=null && picture.Hash != pictureInfo.Hash))
            {
                loadImage(pictureInfo);
            }
            return picture;
        }

        public void Notify(PictureInfoDTO info)
        {
            if(PictureLoaded!=null)
            {
                PictureLoaded(this, new PictureInfoEventArgs(info));
            }
        }
        private void loadImage(PictureInfoDTO picture)
        {
            ServicePool.Add(new PictureDownloadingServiceCommand(picture, this));
        }

        public void AddToCache(PictureCacheItem image)
        {
            Cache.Add(image.PictureId.ToString(), image,CacheItemPriority.Low,null,new SlidingTime(TimeSpan.FromDays(10)));
        }
    }
}
