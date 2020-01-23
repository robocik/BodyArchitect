using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Windows;
using System.Windows.Threading;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using ImageTools;

namespace BodyArchitect.WP7.Controls.Cache
{
    public class PictureDownloadingServiceCommand : IServiceCommand
    {
        private PictureInfoDTO picture;

        public PictureDownloadingServiceCommand(PictureInfoDTO picture)
        {
            this.picture = picture;
        }

        public void Execute()
        {
            try
            {
                var m = new ServiceManager<GetImageCompletedEventArgs>(delegate(BodyArchitectAccessServiceClient client1, EventHandler<GetImageCompletedEventArgs> operationCompleted)
                {
                    using (OperationContextScope ocs = new OperationContextScope(client1.InnerChannel))
                    {
                        OperationContext.Current.OutgoingMessageHeaders.Add(MessageHeader.CreateHeader("SessionId", "http://MYBASERVICE.TK/", ApplicationState.Current.SessionData.Token.SessionId));
                        OperationContext.Current.OutgoingMessageHeaders.Add(MessageHeader.CreateHeader("PictureId", "http://MYBASERVICE.TK/",picture.PictureIdk__BackingField));
                        OperationContext.Current.OutgoingMessageHeaders.Add(MessageHeader.CreateHeader("Hash", "http://MYBASERVICE.TK/",picture.Hashk__BackingField));
                        ApplicationState.AddCustomHeaders();
                        client1.GetImageCompleted -= operationCompleted;
                        client1.GetImageCompleted += operationCompleted;
                        client1.GetImageAsync();
                        
                    }


                });
                m.OperationCompleted += (s, a) =>
                {
                    if (a.Error != null)
                    {
                        PictureCacheItem item = new PictureCacheItem(null, picture.PictureIdk__BackingField, picture.Hashk__BackingField);
                        PicturesCache.Instance.AddToCache(item);
                        PicturesCache.Instance.Notify(new PictureInfoDTO() { PictureIdk__BackingField = picture.PictureIdk__BackingField, Hashk__BackingField = picture.Hashk__BackingField });
                    }
                    else if (a.Result != null)
                    {
                        MemoryStream stream = new MemoryStream(a.Result.Result);
                        ExtendedImage desert = new ExtendedImage();
                        desert.SetSource(stream);
                        desert.LoadingFailed += delegate(object sender, UnhandledExceptionEventArgs e)
                        {
                            PictureCacheItem item = new PictureCacheItem(null, picture.PictureIdk__BackingField, picture.Hashk__BackingField);
                            PicturesCache.Instance.AddToCache(item);
                            PicturesCache.Instance.Notify(new PictureInfoDTO() { PictureIdk__BackingField = picture.PictureIdk__BackingField, Hashk__BackingField = picture.Hashk__BackingField });
                        };
                        desert.LoadingCompleted += delegate
                                                       {
                                                           ApplicationState.SynchronizationContext.Post(delegate
                                                                   {
                                                                       var image = desert.ToBitmap();
                                                                       PictureCacheItem item = new PictureCacheItem(image, picture.PictureIdk__BackingField, picture.Hashk__BackingField);
                                                                       PicturesCache.Instance.AddToCache(item);
                                                                       PicturesCache.Instance.Notify(picture);
                                                                       //if (OperationCompleted != null)
                                                                       //{
                                                                       //    OperationCompleted(this, EventArgs.Empty);
                                                                       //}
                                                                   },null);

                            
                        };

                    }
                };

                m.Run();
                
            }
            catch (Exception)
            {
                PictureCacheItem item = new PictureCacheItem(null, picture.PictureIdk__BackingField, picture.Hashk__BackingField);
                PicturesCache.Instance.AddToCache(item);
                PicturesCache.Instance.Notify(new PictureInfoDTO() { PictureIdk__BackingField = picture.PictureIdk__BackingField, Hashk__BackingField = picture.Hashk__BackingField });
            }


        }

    }
}