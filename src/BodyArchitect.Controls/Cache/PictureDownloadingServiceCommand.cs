using System;
using System.Collections.Generic;
using System.IO;
using BodyArchitect.WCF;
using BodyArchitect.Logger;
using BodyArchitect.Service.Model;

namespace BodyArchitect.Controls.Cache
{
    public class PictureDownloadingServiceCommand : IServiceCommand
    {
        private PictureInfoDTO picture;
        private PicturesCache cache;

        public PictureDownloadingServiceCommand(PictureInfoDTO picture, PicturesCache cache)
        {
            this.picture = picture;
            this.cache = cache;
        }

        public void Execute()
        {
            try
            {
                var image = ServiceManager.GetImage(picture);
                cache.AddToCache(image.ToPictureCacheItem());
                cache.Notify(image.GetPictureInfo());
            }
            catch (FileNotFoundException ex)
            {
                ExceptionHandler.Default.Process(ex);
                PictureCacheItem item = new PictureCacheItem(null, picture.PictureId, picture.Hash);
                cache.AddToCache(item);
                cache.Notify(new PictureInfoDTO(picture.PictureId, picture.Hash));
            }
            
            
        }
    }
}
