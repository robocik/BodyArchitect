using System;
using System.Collections.Generic;
using System.IO;
using BodyArchitect.Client.WCF;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Logger;

namespace BodyArchitect.Client.UI.Cache
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
