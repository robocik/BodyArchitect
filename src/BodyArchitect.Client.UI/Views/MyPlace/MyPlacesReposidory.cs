using System;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Threading;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.WCF;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.UI.Views.MyPlace
{
    public class MyPlacesReposidory : ObjectsCacheBase<MyPlaceDTO>
    {
        private static ConcurrentDictionary<CacheKey, MyPlacesReposidory> dictExercises = new ConcurrentDictionary<CacheKey, MyPlacesReposidory>();
        private CacheKey key;

        private MyPlacesReposidory(CacheKey key)
        {
            this.key = key;
        }

        public static MyPlacesReposidory GetCache(Guid? userId)
        {
            if (userId.HasValue && userId == UserContext.Current.CurrentProfile.GlobalId)
            {
                userId = null;
            }
            var key = new CacheKey() {ProfileId = userId };
            return dictExercises.GetOrAdd(key, (v) => new MyPlacesReposidory(v));
        }

        protected override PagedResult<MyPlaceDTO> GetItemsMethod(PartialRetrievingInfo pageInfo)
        {
            var param=new MyPlaceSearchCriteria();
            param.ProfileId = key.ProfileId;
            return ServiceManager.GetMyPlaces(param, pageInfo);
        }

        
    }
}
