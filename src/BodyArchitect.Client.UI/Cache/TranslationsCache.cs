using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Caching;
using Microsoft.Practices.EnterpriseLibrary.Caching.Expirations;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace BodyArchitect.Client.UI.Cache
{
    public class TranslationsCache
    {
        public const string TranslationsCacheName = "TranslationCacheManager";
        public const string SupportedLanguagesKey = "SupportedLanguages";
        private ICacheManager cache;
        private object syncObject = new object();
        private static TranslationsCache translationsCache;

        public static TranslationsCache Instance
        {
            get
            {
                if (translationsCache == null)
                {
                    translationsCache = new TranslationsCache();
                }
                return translationsCache;
            }
        }

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

                            cache = EnterpriseLibraryContainer.Current.GetInstance<ICacheManager>(TranslationsCacheName);
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

        public string Get(string key)
        {
            return (string) Cache.GetData(key);
        }

        public string[] GetSupportedLanguages()
        {
            return (string[])Cache.GetData(SupportedLanguagesKey);
        }

        public void SetSupportedLanguages(string[] supportedLanguages)
        {//refresh supported languages after 3 months
            Cache.Add(SupportedLanguagesKey,supportedLanguages,CacheItemPriority.Normal,null,new AbsoluteTime(DateTime.Now.AddMonths(3)));
        }

        public void Remove(string key)
        {
            Cache.Remove(key);
        }

        public void Add(string key,string translation)
        {
            Cache.Add(key, translation);
        }
    }
}
