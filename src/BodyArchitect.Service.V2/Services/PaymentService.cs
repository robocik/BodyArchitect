using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using AutoMapper;
using BodyArchitect.DataAccess.NHibernate;
using BodyArchitect.Model;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Shared;
using NHibernate;
using NHibernate.Transform;
using Profile = BodyArchitect.Model.Profile;

namespace BodyArchitect.Service.V2.Services
{
    public class PaymentService :ServiceBase
    {
        public PaymentService(ISession session, SecurityInfo securityInfo, ServiceConfiguration configuration)
            : base(session, securityInfo, configuration)
        {
        }

        public PaymentBasketDTO PaymentBasketOperation(PaymentBasketDTO koszyk)
        {
            if (koszyk.GlobalId !=Guid.Empty)
            {
                throw new InvalidOperationException("This koszyk is already paid");
            }
            decimal totalSum = 0;
            var db = koszyk.Map<PaymentBasket>();
            using (var trans = Session.BeginSaveTransaction())
            {
                var dbProfile = Session.Load<Profile>(SecurityInfo.SessionData.Profile.GlobalId);
                db.Customer = Session.Get<Customer>(koszyk.CustomerId);
                if (db.Customer != null && db.Customer.Profile != dbProfile)
                {
                    throw new CrossProfileOperationException();
                }
                db.DateTime = Configuration.TimerService.UtcNow;
                db.Profile = dbProfile;
                foreach (var zakupDto in koszyk.Payments)
                {
                    var zakup = zakupDto.Map<Payment>();
                    zakup.Product = Session.Get<Product>(zakupDto.Product.GlobalId);

                    if (zakup.Product.Profile !=dbProfile)
                    {
                        throw new CrossProfileOperationException();
                    }

                    if (zakup.Product.Payment != null)
                    {//gdy po pobraniu produktu on juz ma jakis Zakup to znaczy ze jest już zapłacony!
                        throw new ProductAlreadyPaidException();
                    }
                    if(zakup.DateTime==DateTime.MinValue)
                    {
                        zakup.DateTime = Configuration.TimerService.UtcNow;
                    }
                    zakup.PaymentBasket = db;
                    zakup.Product.Payment = zakup;
                    db.Payments.Add(zakup);
                    totalSum += zakup.Count * zakup.Product.Price;
                }
                if (totalSum != koszyk.TotalPrice)
                {
                    throw new ConsistencyException();
                }
                db = Session.Merge(db);
                trans.Commit();
            }

            return db.Map< PaymentBasketDTO>();
        }

        public PagedResult<PaymentBasketDTO> GetPaymentBaskets(GetPaymentBasketParam getPaymentBasketParam, PartialRetrievingInfo retrievingInfo)
        {
            using (var trans = Session.BeginGetTransaction())
            {
                Profile dbProfile = Session.Load<Profile>(SecurityInfo.SessionData.Profile.GlobalId);

                var query = Session.QueryOver<PaymentBasket>().Where(x => x.Profile == dbProfile);


                if (getPaymentBasketParam.StartTime.HasValue)
                {
                    query = query.Where(x => x.DateTime >= getPaymentBasketParam.StartTime.Value);
                }
                if (getPaymentBasketParam.EndTime.HasValue)
                {
                    query = query.Where(x => x.DateTime <= getPaymentBasketParam.EndTime.Value);
                }
                if (getPaymentBasketParam.CustomerId.HasValue)
                {
                    Customer dbCustomer = Session.Load<Customer>(getPaymentBasketParam.CustomerId.Value);
                    query = query.Where(x => x.Customer == dbCustomer);
                }
                query.TransformUsing(new DistinctRootEntityResultTransformer());
                trans.Commit();
                return query.ToPagedResults<PaymentBasketDTO, PaymentBasket>(retrievingInfo);
            }
        }
    }
}
