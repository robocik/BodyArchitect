using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.WCF;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;

namespace BodyArchitect.Client.UI
{
    public class MyTrainingsReposidory : ObjectsCacheBase<MyTrainingDTO>
    {
        private static ConcurrentDictionary<CacheKey, MyTrainingsReposidory> dictExercises = new ConcurrentDictionary<CacheKey, MyTrainingsReposidory>();
        private CacheKey key;

        private MyTrainingsReposidory(CacheKey key)
        {
            this.key = key;
        }

        //public static MyTrainingsReposidory Instance
        //{
        //    get { return GetCache(null,null); }
        //}

        public static MyTrainingsReposidory GetCache(Guid? customerId,Guid? userId)
        {
            if (userId.HasValue && userId == UserContext.Current.CurrentProfile.GlobalId)
            {
                userId = null;
            }
            var key = new CacheKey(){CustomerId = customerId,ProfileId = userId};
            return dictExercises.GetOrAdd(key, (v) => new MyTrainingsReposidory(v));
        }

        protected override PagedResult<MyTrainingDTO> GetItemsMethod(PartialRetrievingInfo pageInfo)
        {
            GetMyTrainingsParam param = new GetMyTrainingsParam();
            param.CustomerId = key.CustomerId;
            param.UserId = key.ProfileId;
            return ServiceManager.GetMyTrainings(param,new PartialRetrievingInfo());
        }
    }
}
