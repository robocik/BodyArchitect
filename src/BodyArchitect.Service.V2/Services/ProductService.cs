using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using BodyArchitect.DataAccess.NHibernate;
using BodyArchitect.Logger;
using BodyArchitect.Model;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Shared;
using NHibernate;
using NHibernate.Criterion.Lambda;
using NHibernate.Transform;
using Profile = BodyArchitect.Model.Profile;

namespace BodyArchitect.Service.V2.Services
{
    class ProductService:ServiceBase
    {
        public ProductService(ISession session, SecurityInfo securityInfo, ServiceConfiguration configuration)
            : base(session, securityInfo, configuration)
        {
        }

        public PagedResult<ProductInfoDTO> GetProducts(GetProductsParam getProductsParam, PartialRetrievingInfo retrievingInfo)
        {
            Log.WriteWarning("GetProducts:Username={0}", SecurityInfo.SessionData.Profile.UserName);
            if (!SecurityInfo.Licence.IsInstructor)
            {
                throw new LicenceException("This feature is allowed for Instructor account");
            }

            using (var transactionScope = Session.BeginGetTransaction())
            {
                var dbProfile = Session.Load<Profile>(SecurityInfo.SessionData.Profile.GlobalId);
                var queryProducts = Session.QueryOver<Product>().Where(x => x.Profile == dbProfile).Fetch(x => x.Payment).Eager;

                if (getProductsParam.StartTime.HasValue)
                {
                    queryProducts = queryProducts.Where(day => day.DateTime >= getProductsParam.StartTime);
                }
                if (getProductsParam.EndTime.HasValue)
                {
                    queryProducts = queryProducts.Where(day => day.DateTime <= getProductsParam.EndTime);
                }
                if(getProductsParam.CustomerId.HasValue)
                {
                    var dbCustomer = Session.Get<Customer>(getProductsParam.CustomerId.Value);
                    if(dbCustomer.Profile!=dbProfile)
                    {
                        throw new CrossProfileOperationException("Customer belongs to another user");
                    }
                    queryProducts = queryProducts.Where(day => day.Customer == dbCustomer);
                }
                switch (getProductsParam.PaymentCriteria)
                {
                    case PaymentCriteria.WithoutPayment:
                        queryProducts = queryProducts.Where(day => day.Payment==null);
                        break;
                    case PaymentCriteria.WithPayment:
                        queryProducts = queryProducts.Where(day => day.Payment != null);
                        break;
                }
                IQueryOverOrderBuilder<Product, Product> orderBuilder;
                switch (getProductsParam.SortOrder)
                {
                    case ProductsSortOrder.ByName:
                        orderBuilder = queryProducts.OrderBy(x => x.Name);
                        break;
                    case ProductsSortOrder.ByPaid:
                        orderBuilder = queryProducts.OrderBy(x => x.Payment);
                        break;
                    default:
                        orderBuilder = queryProducts.OrderBy(x => x.DateTime);
                        break;
                }
                if (getProductsParam.SortAscending)
                {
                    queryProducts = orderBuilder.Asc;
                }
                else
                {
                    queryProducts = orderBuilder.Desc;
                }

                //queryProducts = queryProducts.TransformUsing(Transformers.DistinctRootEntity);
                var listPack = queryProducts.ToPagedResults<ProductInfoDTO, Product>(retrievingInfo);
                transactionScope.Commit();
                return listPack;
            }
        }
    }
}
