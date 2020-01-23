using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using AutoMapper;
using BodyArchitect.Logger;
using BodyArchitect.Model;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Reports;
using BodyArchitect.Service.V2.Services;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WCF;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Criterion.Lambda;
using NHibernate.Transform;
using AccountType = BodyArchitect.Model.AccountType;
using Privacy = BodyArchitect.Model.Privacy;
using Profile = BodyArchitect.Model.Profile;
using ValidationResult = BodyArchitect.Portable.Exceptions.ValidationResult;

namespace BodyArchitect.Service.V2
{
    public static class ServiceHelper
    {
        public static IList<ValidationResult> ToValidationResults(this ValidationResults fault)
        {
            List<ValidationResult> result = new List<ValidationResult>();
            foreach (var detail in fault)
            {
                result.Add(new ValidationResult(detail.Message, detail.Key, detail.Tag));
            }
            return result;
        }
        public static TRes Map<T, TRes>(this T obj)
        {
            return Mapper.Map<T, TRes>(obj);
        }

        public static TRes Map<TRes>(this object obj)
        {
            return Mapper.Map<TRes>(obj);
        }

        public static T GetRealObject<T>(this ISession session,object obj)
        {
            return (T)session.GetSessionImplementation().PersistenceContext.Unproxy(obj);
        }

        public static bool IsPrivateCalendar(Profile dbProfile, Profile dbMyProfile)
        {
            return (dbProfile != dbMyProfile && dbProfile.Licence.AccountType != AccountType.User &&
                    (dbProfile.Privacy.CalendarView == Privacy.Private ||
                     (dbProfile.Privacy.CalendarView == Privacy.FriendsOnly && !dbProfile.Friends.Contains(dbMyProfile))));
        }

        public static bool IsPrivateSizes(Profile dbProfile, Profile dbMyProfile)
        {
            return (dbProfile != dbMyProfile && dbProfile.Licence.AccountType != AccountType.User &&
                    (dbProfile.Privacy.Sizes == Privacy.Private ||
                     (dbProfile.Privacy.Sizes == Privacy.FriendsOnly && !dbProfile.Friends.Contains(dbMyProfile))));
        }

        public static IQueryOver<T, T> ApplyUser<T>(this IQueryOver<T, T> query, IUserParameter param, ISession session, SecurityInfo securityInfo) where T : IHasUser, IHasCustomer
        {
            var dbMyProfile = session.Load<Profile>(securityInfo.SessionData.Profile.GlobalId);
            var dbProfile = dbMyProfile;
            if (param.UserId.HasValue)
            {
                dbProfile = session.Get<Profile>(param.UserId.Value);
            }

            if (param.CustomerId != null && dbProfile.GlobalId != securityInfo.SessionData.Profile.GlobalId)
            {
                throw new CrossProfileOperationException("This customer doesn't belong to your profile");
            }
            //privacy settings have effect only for paid accounts. Free users have always public calendar
            if (IsPrivateCalendar(dbProfile,dbMyProfile))
            {
                return null;
            }

            query = query.Where(x => x.Profile == dbProfile);
            if (param.CustomerId.HasValue)
            {
                Customer customer = session.Get<Customer>(param.CustomerId.Value);
                if (customer.Profile != dbProfile)
                {
                    throw new CrossProfileOperationException("This customer doesn't belong to your profile");
                }
                query = query.Where(day => day.Customer == customer);
            }
            else
            {
                query = query.Where(day => day.Customer == null);
            }
            return query;
        }

        public static IQueryOver<T, T> ApplySorting<T>(this IQueryOver<T, T> query, SearchSortOrder sortOrder, bool sortAscending) where T : ISortable
        {
            IQueryOverOrderBuilder<T, T> orderBuilder = null;
            if (sortOrder == SearchSortOrder.Newest)
            {
                orderBuilder = query.OrderBy(x => x.CreationDate);
            }
            else if (sortOrder == SearchSortOrder.HighestRating)
            {
                orderBuilder = query.OrderBy(x => x.Rating);
            }
            else
            {
                orderBuilder = query.OrderBy(x => x.Name);
            }

            if (sortAscending)
            {
                query = orderBuilder.Asc;
            }
            else
            {
                query = orderBuilder.Desc;
            }
            return query;
        }

        public static PagedResult<T> ToPagedResult<T>(this IList<T> list, PartialRetrievingInfo retrievingInfo)
        {
            IList<T> partialList = null;
            if (retrievingInfo.PageSize > PartialRetrievingInfo.AllElementsPageSize)
            {
                if (retrievingInfo.PageSize > PartialRetrievingInfo.AllElementsPageSize)
                {
                    partialList = list.Skip(retrievingInfo.PageIndex * retrievingInfo.PageSize).Take(retrievingInfo.PageSize).ToList();
                }
                else
                {
                    partialList = list;
                }
            }
            else
            {
                partialList = list;
            }
            return new PagedResult<T>(partialList, list.Count, retrievingInfo.PageIndex);
        }

        static public IQueryOver<Model> ApplyPaging<Model>(this IQueryOver<Model, Model> query, PartialRetrievingInfo retrievingInfo)
        {
            if (retrievingInfo.PageSize > PartialRetrievingInfo.AllElementsPageSize)
            {
                return query.Take(retrievingInfo.PageSize).Skip(retrievingInfo.PageIndex*retrievingInfo.PageSize);
            }
            else
            {
                return query;
            }
        }

        static public PagedResult<DTO> ToPagedResults<DTO, Model>(this IQueryOver<Model, Model> query, PartialRetrievingInfo retrievingInfo, string countPropertyName = null, Func<IEnumerable<Model>, DTO[]> mappingMethod = null, IQueryOver<Model, Model> fetchQuery=null)
        {
            IEnumerable<Model> queryEnumerable = null;
            if (fetchQuery != null)
            {
                fetchQuery.ApplyPaging(retrievingInfo).Future();
            }

            
            int count = 0;
            if (retrievingInfo.PageSize > PartialRetrievingInfo.AllElementsPageSize)
            {
                IFutureValue<int> rowCountQuery = query.ToRowCountQuery().FutureValue<int>(); 
                IQueryOver<Model> pagedResults = query.ApplyPaging(retrievingInfo);
                
                queryEnumerable = pagedResults.Future();
                count = rowCountQuery.Value;
            }
            else
            {
                queryEnumerable = query.Future();
                count = queryEnumerable.Count();
            }

            DTO[] list = null;
            if (mappingMethod == null)
            {
                list = Mapper.Map<IEnumerable<Model>, DTO[]>(queryEnumerable);
            }
            else
            {
                list = mappingMethod(queryEnumerable);
            }
            PagedResult<DTO> res = new PagedResult<DTO>(list, count, retrievingInfo.PageIndex);
            res.RetrievedDateTime = DateTime.UtcNow;
            Log.WriteInfo("Paged result. AllCount:{0},PageIndex:{1},PageSize:{2}", res.AllItemsCount, res.PageIndex, res.Items.Count);
            return res;
        }


        static public PagedResult<DTO> ToExPagedResults<DTO, Model>(this IQueryOver<Model, Model> query, PartialRetrievingInfo retrievingInfo,  IQueryOver<Model, Model> idQuery, Func<IEnumerable<Model>, DTO[]> mappingMethod = null)
            where Model:FMGlobalObject
        {
            
            IEnumerable<Model> queryEnumerable = null;
            int count = 0;

            query = query.TransformUsing(Transformers.DistinctRootEntity);
            if (retrievingInfo.PageSize > PartialRetrievingInfo.AllElementsPageSize)
            {
                ICriteria countCriteria = CriteriaTransformer.Clone(idQuery.RootCriteria);
                countCriteria.SetProjection(Projections.CountDistinct("GlobalId"));
                IFutureValue<int> rowCountQuery = countCriteria.FutureValue<int>();

                idQuery = (QueryOver<Model, Model>)idQuery.ApplyPaging(retrievingInfo);
                var ids = idQuery.Select(Projections.Distinct(Projections.Property("GlobalId"))).Future<Guid>();
                count = rowCountQuery.Value;

                query = query.WhereRestrictionOn(x => x.GlobalId).IsIn(ids.ToList());
                queryEnumerable = query.List();

            }
            else
            {
                var ids = idQuery.Select(Projections.Distinct(Projections.Property("GlobalId"))).Future<Guid>();
                query = query.WhereRestrictionOn(x => x.GlobalId).IsIn(ids.ToList());
                queryEnumerable = query.List();
                count = queryEnumerable.Count();
            }

            DTO[] list = null;
            if (mappingMethod == null)
            {
                var temp = queryEnumerable.ToList();
                list = Mapper.Map<IEnumerable<Model>, DTO[]>(temp);
            }
            else
            {
                list = mappingMethod(queryEnumerable);
            }
            PagedResult<DTO> res = new PagedResult<DTO>(list, count, retrievingInfo.PageIndex);
            res.RetrievedDateTime = DateTime.UtcNow;
            Log.WriteInfo("Paged result. AllCount:{0},PageIndex:{1},PageSize:{2}", res.AllItemsCount, res.PageIndex, res.Items.Count);
            return res;
        }

        public static CultureInfo GetCultureFromCountry(int countryId)
        {
            var country = Country.GetCountry(countryId);
            CultureInfo culture = CultureInfo.CurrentUICulture;
            if (country != null)
            {
                try
                {
                    culture = new CultureInfo(country.RegionInfo.TwoLetterISORegionName);
                }
                catch (Exception)
                {
                    //ExceptionHandler.Default.Process(ex);
                }

            }
            return culture;
        }


        public static CultureInfo GetProfileCulture(this Profile dbProfile)
        {
            return GetCultureFromCountry(dbProfile.CountryId);
        }
    }
}
