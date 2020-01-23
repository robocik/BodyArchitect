using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.DataAccess.NHibernate;
using BodyArchitect.Logger;
using BodyArchitect.Model;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Shared;
using NHibernate;
using NHibernate.Criterion.Lambda;

namespace BodyArchitect.Service.V2.Services
{
    class MyPlaceService : ServiceBase
    {
        public MyPlaceService(ISession session, SecurityInfo securityInfo, ServiceConfiguration configuration)
            : base(session, securityInfo, configuration)
        {
        }

        public MyPlaceDTO SaveMyPlace(MyPlaceDTO place)
        {
            Log.WriteWarning("SaveMyPlace:Username={0},place={1}", SecurityInfo.SessionData.Profile.UserName,place.GlobalId);

            if (!SecurityInfo.Licence.IsPremium)
            {
                throw new LicenceException("This feature is allowed for Premium account");
            }
            var db = place.Map<MyPlace>();
            using (var transactionScope = Session.BeginSaveTransaction())
            {
                Address oldAddress = null;
                Profile dbProfile = Session.Load<Profile>(SecurityInfo.SessionData.Profile.GlobalId);
                if (db.IsNew)
                {
                    db.CreationDate = Configuration.TimerService.UtcNow;
                    
                }
                else
                {
                    var temp = Session.Get<MyPlace>(db.GlobalId);
                    if (temp != null)
                    {
                        if (dbProfile != temp.Profile)
                        {
                            throw new CrossProfileOperationException("Cannot modify MyPlace for another user");
                        }
                        if(temp.IsSystem!=db.IsSystem)
                        {
                            throw new InvalidOperationException("Cannot change IsSystem property");
                        }
                        if(temp.IsDefault!=db.IsDefault)
                        {//user changed IsDefault
                            throw new InvalidOperationException("Cannot change IsDefault property using this method. Use MyPlaceOperation instead");
                        }
                        oldAddress = temp.Address;
                    }
                }

                if (db.Address != null && db.Address.IsEmpty)
                {
                    Log.WriteVerbose("Address is empty.");
                    if (!db.Address.IsNew && (oldAddress == null || db.Address.GlobalId != oldAddress.GlobalId))
                    {
                        Log.WriteInfo("Delete Address from db");
                        Session.Delete(db.Address);
                    }
                    db.Address = null;
                }

                if (oldAddress != null && (db.Address == null || oldAddress.GlobalId != db.Address.GlobalId))
                {
                    Session.Delete(oldAddress);
                }
                if (string.IsNullOrEmpty(db.Color))
                {
                    db.Color = Constants.DefaultColor;
                }
                db.Profile = dbProfile;
                db=Session.Merge(db);
                dbProfile.DataInfo.MyPlaceHash = Guid.NewGuid();
                transactionScope.Commit();
                return db.Map<MyPlaceDTO>();
            }
        }

        public MyPlaceDTO MyPlaceOperation(MyPlaceOperationParam param)
        {
            Log.WriteWarning("MyPlaceOperation:Username={0},place={1},operation={2}", SecurityInfo.SessionData.Profile.UserName, param.MyPlaceId,param.Operation);

            if (!SecurityInfo.Licence.IsPremium)
            {
                throw new LicenceException("This feature is allowed for Premium account");
            }

            using (var tx = Session.BeginSaveTransaction())
            {
                var dbGym = Session.Get<MyPlace>(param.MyPlaceId);
                if (SecurityInfo.SessionData.Profile.GlobalId != dbGym.Profile.GlobalId)
                {
                    throw new CrossProfileOperationException("Cannot modify MyPlace for another user");
                }
                if (param.Operation == MyPlaceOperationType.Delete)
                {
                    if (dbGym.IsSystem)
                    {
                        throw new InvalidOperationException("Cannot delete system place");
                    }
                    if (dbGym.Entries.Count > 0)
                    {
                        throw new DeleteConstraintException("This Gym contains strenght training entries");
                    }
                    Session.Delete(dbGym);

                    if(dbGym.IsDefault)
                    {//we removed default place so now system place should be marked as default
                        var systemPlace=Session.QueryOver<MyPlace>().Where(x => x.Profile.GlobalId == SecurityInfo.SessionData.Profile.GlobalId && x.IsSystem).SingleOrDefault();
                        systemPlace.IsDefault = true;
                        Session.Update(systemPlace);
                    }
                    dbGym = null;
                }
                else
                {
                    setIsDefault(dbGym);
                }
                var dbProfile = Session.Get<Profile>(SecurityInfo.SessionData.Profile.GlobalId);
                dbProfile.DataInfo.MyPlaceHash = Guid.NewGuid();
                tx.Commit();

                if (dbGym != null)
                {
                    return dbGym.Map<MyPlaceDTO>();
                }
                return null;
            }
            
        }

        void setIsDefault(MyPlace newDefaultPlace)
        {
            var defaultPlace = Session.QueryOver<MyPlace>().Where(x => x.Profile.GlobalId == SecurityInfo.SessionData.Profile.GlobalId && x.IsDefault).SingleOrDefault();
            defaultPlace.IsDefault = false;
            newDefaultPlace.IsDefault = true;
            Session.Update(defaultPlace);
        }

        public PagedResult<MyPlaceDTO> GetMyPlaces(MyPlaceSearchCriteria searchCriteria, PartialRetrievingInfo pageInfo)
        {
            Log.WriteWarning("GetMyPlaces:Username={0}", SecurityInfo.SessionData.Profile.UserName);

            using (var transactionScope = Session.BeginGetTransaction())
            {
                var dbProfile = Session.Load<Profile>(SecurityInfo.SessionData.Profile.GlobalId);
                if(searchCriteria.ProfileId.HasValue)
                {
                    dbProfile = Session.Load<Profile>(searchCriteria.ProfileId.Value);
                }
                var query = Session.QueryOver<MyPlace>().Where(x => x.Profile == dbProfile).Fetch(x=>x.Address).Eager;

                IQueryOverOrderBuilder<MyPlace, MyPlace> orderBuilder;
                switch (searchCriteria.SortOrder)
                {
                    case MyPlaceSortOrder.CreationDate:
                        orderBuilder = query.OrderBy(x => x.CreationDate);
                        break;
                    default:
                        orderBuilder = query.OrderBy(x => x.Name);
                        break;
                }
                if (searchCriteria.SortAscending)
                {
                    query = orderBuilder.Asc;
                }
                else
                {
                    query = orderBuilder.Desc;
                }

                //queryCustomer = queryCustomer.TransformUsing(Transformers.DistinctRootEntity);
                var listPack = query.ToPagedResults<MyPlaceDTO, MyPlace>(pageInfo);
                transactionScope.Commit();
                return listPack;
            }
        }
    }

    
}
