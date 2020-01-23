using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.WP7.Controls.Cache
{
    public class PictureInfoEventArgs : EventArgs
    {
        public PictureInfoEventArgs(PictureInfoDTO pictureInfo)
        {
            PictureInfo = pictureInfo;
        }

        public PictureInfoDTO PictureInfo { get; private set; }
    }

    public class PictureCacheItem
    {
        public PictureCacheItem(BitmapSource image, Guid picureId, string hash)
        {
            Image = image;
            PictureId = picureId;
            Hash = hash;
        }

        public BitmapSource Image { get; set; }
        public Guid PictureId { get; set; }
        public string Hash { get;  set; }


    }

    public class PictureItemStore
    {
        public int Width { get; set; }

        public int Height { get; set; }

        public byte[] Pixels { get; set; }

        public string Hash { get; set; }

        public Guid PictureId { get; set; }

        public TimeSpan ValidDuration { get; set; }

        public DateTime CreationDate{ get; set; }
    }

    public class PicturesCache
    {
        public event EventHandler<PictureInfoEventArgs> PictureLoaded;
        public const string FileName = "pictures.cache";
        static PicturesCache picturesCache;
        Dictionary<Guid,PictureInfoDTO> downloadingPictures = new Dictionary<Guid, PictureInfoDTO>();

        private PicturesCache()
        {
        }

        public Cache Cache
        {
            get { return CacheManager.PictureCache; }
        }

        public void RemoveFile()
        {
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                try
                {
                    Cache.CacheItems.Clear();
                    if (store.FileExists(FileName))
                    {
                        store.DeleteFile(FileName);
                    }
                }
                catch (IsolatedStorageException )
                {
                }
            }
        }

        public static void Load()
        {
            picturesCache = new PicturesCache();
            try
            {
                using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (store.FileExists(FileName))
                    {
                        using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(FileName, FileMode.Open, store))
                        {
                            DataContractSerializer serializer = new DataContractSerializer(typeof(List<PictureItemStore>));
                            List<PictureItemStore> items = (List<PictureItemStore>)serializer.ReadObject(stream);

                            foreach (var itemStore in items)
                            {
                                WriteableBitmap _bitmap = new WriteableBitmap(itemStore.Width, itemStore.Height);
                                int count = _bitmap.Pixels.Length * sizeof(int);
                                Buffer.BlockCopy(itemStore.Pixels, 0, _bitmap.Pixels, 0, count);
                                PictureCacheItem picItem = new PictureCacheItem(_bitmap, itemStore.PictureId, itemStore.Hash);
                                CacheItem item = new CacheItem(picItem, itemStore.ValidDuration, itemStore.CreationDate);
                                picturesCache.Cache.Add(itemStore.PictureId.ToString(), item);
                            }

                        }
                    }
                }
            }
            catch
            {
                
            }
           
        }
        public void Save()
        {
            if (CacheManager.PictureCache.CacheItems.Values.Count==0)
            {
                return;
            }
            try
            {
                using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(FileName, FileMode.Create, store))
                    {
                        List<PictureItemStore> items = new List<PictureItemStore>();

                        foreach (var item in CacheManager.PictureCache.CacheItems.Values)
                        {
                            PictureItemStore storeItem = new PictureItemStore();
                            PictureCacheItem cacheItem = (PictureCacheItem)item.Data;
                            WriteableBitmap _bitmap = (WriteableBitmap)cacheItem.Image;
                            if (cacheItem.Image == null)
                            {
                                continue;
                            }
                            storeItem.Width = cacheItem.Image.PixelWidth;
                            storeItem.Height = cacheItem.Image.PixelHeight;
                            storeItem.PictureId = cacheItem.PictureId;
                            storeItem.ValidDuration = item.ValidDuration;
                            storeItem.CreationDate = item.CreationDate;
                            storeItem.Hash = cacheItem.Hash;
                            int count = _bitmap.Pixels.Length * sizeof(int);
                            storeItem.Pixels = new byte[count];
                            Buffer.BlockCopy(_bitmap.Pixels, 0, storeItem.Pixels, 0, count);
                            items.Add(storeItem);
                        }
                        DataContractSerializer serializer = new DataContractSerializer(typeof(List<PictureItemStore>));
                        serializer.WriteObject(stream, items);
                    }
                }
            }
            catch (Exception)
            {
            }
            
        }

        public static PicturesCache Instance
        {
            get
            {
                if (picturesCache == null)
                {
                    picturesCache = new PicturesCache();
                }
                return picturesCache;
            }
        }


        public void Remove(Guid pictureId)
        {
            CacheManager.PictureCache.Remove(pictureId.ToString());
        }

        public PictureCacheItem GetImage(PictureInfoDTO pictureInfo)
        {

            PictureCacheItem picture = (PictureCacheItem)CacheManager.PictureCache.Get(pictureInfo.PictureIdk__BackingField.ToString());

            if (picture == null || (picture.Image != null && picture.Hash != pictureInfo.Hashk__BackingField))
            {
                loadImage(pictureInfo);
            }
            return picture;
        }

        public void Notify(PictureInfoDTO info)
        {
            if (PictureLoaded != null)
            {
                PictureLoaded(this, new PictureInfoEventArgs(info));
            }
        }
        private void loadImage(PictureInfoDTO picture)
        {
            if (!downloadingPictures.ContainsKey(picture.PictureIdk__BackingField))
            {
                downloadingPictures.Add(picture.PictureIdk__BackingField,picture);
                ServicePool.Add(new PictureDownloadingServiceCommand(picture));
            }
        }

        public void AddToCache(PictureCacheItem image)
        {
            CacheManager.PictureCache.Add(image.PictureId.ToString(), image,TimeSpan.FromDays(10));
            if (downloadingPictures.ContainsKey(image.PictureId))
            {
                downloadingPictures.Remove(image.PictureId);
            }
        }
    }
}