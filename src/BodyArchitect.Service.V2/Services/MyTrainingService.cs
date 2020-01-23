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
using NHibernate.Criterion;
using NHibernate.Criterion.Lambda;
using NHibernate.Transform;
using TrainingEnd = BodyArchitect.Model.TrainingEnd;

namespace BodyArchitect.Service.V2.Services
{
    class MyTrainingService:ServiceBase
    {
        public MyTrainingService(ISession session, SecurityInfo securityInfo, ServiceConfiguration configuration)
            : base(session, securityInfo, configuration)
        {
        }

        public PagedResult<MyTrainingDTO> GetMyTrainings(GetMyTrainingsParam param, PartialRetrievingInfo retrievingInfo)
        {
            Log.WriteWarning("GetMyTrainings:Username={0}", SecurityInfo.SessionData.Profile.UserName);

            using (var transactionScope = Session.BeginGetTransaction())
            {
                
                MyTraining _temp = null;
                var idQuery = Session.QueryOver<MyTraining>(() => _temp);
                idQuery = (QueryOver<MyTraining, MyTraining>)getMyTrainingsCriterias(idQuery, param);
                if (idQuery == null)
                {
                    return new PagedResult<MyTrainingDTO>(new List<MyTrainingDTO>(), 0, 0);
                }

                var fetchQuery = Session.QueryOver<MyTraining>(()=>_temp)
                    .Fetch(x => x.EntryObjects).Eager
                    .Fetch(x => ((SuplementsEntry)x.EntryObjects.First()).Items).Eager
                    .Fetch(x => (((SuplementsEntry)x.EntryObjects.First()).Items).First().Suplement).Eager
                    .Fetch(x => ((StrengthTrainingEntry)x.EntryObjects.First()).Entries).Eager
                    .Fetch(x => (((StrengthTrainingEntry)x.EntryObjects.First()).Entries).First().Exercise).Eager
                    .Fetch(x => x.EntryObjects.First().LoginData).Eager
                    .Fetch(x => x.EntryObjects.First().LoginData.ApiKey).Eager
                    .Fetch(x => x.EntryObjects.First().Reminder).Eager
                    .Fetch(x => x.EntryObjects.First().TrainingDay).Eager
                    .Fetch(x => x.EntryObjects.First().TrainingDay.Objects).Lazy;
                
                var listPack = fetchQuery.ToExPagedResults<MyTrainingDTO, MyTraining>(retrievingInfo, idQuery);
                transactionScope.Commit();
                return listPack;
            }
        }

        private IQueryOver<MyTraining, MyTraining> getMyTrainingsCriterias(IQueryOver<MyTraining, MyTraining> queryProducts, GetMyTrainingsParam param)
        {
            queryProducts = queryProducts.ApplyUser(param, Session, SecurityInfo);
            if(queryProducts==null)
            {
                return null;
            }

            if(param.MyTrainingId.HasValue)
            {
                queryProducts = queryProducts.Where(day => day.GlobalId == param.MyTrainingId.Value);
            }
            if (param.StartDate.HasValue)
            {
                queryProducts = queryProducts.Where(day => day.StartDate >= param.StartDate);
            }
            if (param.EndDate.HasValue)
            {
                queryProducts = queryProducts.Where(day => day.StartDate <= param.EndDate);
            }
                

            if (param.TrainingEnds.Count > 0)
            {
                var langOr = Restrictions.Disjunction();
                foreach (var lang in param.TrainingEnds)
                {
                    langOr.Add<MyTraining>(x => x.TrainingEnd== (TrainingEnd)lang);
                }
                queryProducts = queryProducts.And(langOr);

            }

            IQueryOverOrderBuilder<MyTraining, MyTraining> orderBuilder;
            switch (param.SortOrder)
            {
                case MyTrainingSortOrder.StartDate:
                    orderBuilder = queryProducts.OrderBy(x => x.StartDate);
                    break;
                case MyTrainingSortOrder.PercentageCompleted:
                    orderBuilder = queryProducts.OrderBy(x => x.PercentageCompleted);
                    break;
                default:
                    orderBuilder = queryProducts.OrderBy(x => x.Name);
                    break;
            }
            if (param.SortAscending)
            {
                queryProducts = orderBuilder.Asc;
            }
            else
            {
                queryProducts = orderBuilder.Desc;
            }
            return queryProducts;
        }
    }
}
