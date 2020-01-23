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
using NHibernate.Criterion;
using NHibernate.Transform;
using CustomerGroupRestrictedType = BodyArchitect.Model.CustomerGroupRestrictedType;
using Gender = BodyArchitect.Model.Gender;
using Profile = BodyArchitect.Model.Profile;

namespace BodyArchitect.Service.V2.Services
{
    public class CustomerGroupService: ServiceBase
    {
        public CustomerGroupService(ISession session, SecurityInfo securityInfo, ServiceConfiguration configuration)
            : base(session, securityInfo, configuration)
        {
        }

        public CustomerGroupDTO SaveCustomerGroup(CustomerGroupDTO customerGroup)
        {
            Log.WriteWarning("SaveCustomerGroup:Username={0},GlobalId={1}", SecurityInfo.SessionData.Profile.UserName, customerGroup.GlobalId);

            if (!SecurityInfo.Licence.IsInstructor)
            {
                throw new LicenceException("This feature is allowed for Instructor account");
            }
            var db = customerGroup.Map<CustomerGroup>();
            using (var trans = Session.BeginSaveTransaction())
            {
                //Profile dbProfile = Session.Load<Profile>(SecurityInfo.SessionData.Profile.Id);
                //CustomerGroup db = null;
                //if (customerGroup.GlobalId != Constants.UnsavedGlobalId)
                //{
                //    db = Session.Get<CustomerGroup>(customerGroup.GlobalId);
                //    Mapper.Map(customerGroup, db);
                //}
                //else
                //{
                //    db = customerGroup.Map<CustomerGroup>();
                //    db.CreationDate = Configuration.TimerService.UtcNow;
                //    db.Profile = dbProfile;
                //}

                //if (SecurityInfo.SessionData.Profile.Id != db.Profile.Id)
                //{
                //    throw new CrossProfileOperationException("Cannot modify customer group for another user");
                //}
                Profile dbProfile = Session.Load<Profile>(SecurityInfo.SessionData.Profile.GlobalId);

                if (db.IsNew)
                {
                    db.CreationDate = Configuration.TimerService.UtcNow;
                }
                else
                {
                    var dbGroup = Session.Get<CustomerGroup>(db.GlobalId);
                    if (dbGroup != null)
                    {
                        if (dbProfile != dbGroup.Profile)
                        {
                            throw new CrossProfileOperationException("Cannot modify Group for another user");
                        }
                    }
                }


                db.Profile = dbProfile;

                if(string.IsNullOrEmpty(db.Color))
                {
                    db.Color = Constants.DefaultColor;
                }

                if(customerGroup.DefaultActivityId.HasValue)
                {
                    db.DefaultActivity = Session.Get<Activity>(customerGroup.DefaultActivityId.Value);
                    if (SecurityInfo.SessionData.Profile.GlobalId != db.DefaultActivity.Profile.GlobalId)
                    {
                        throw new CrossProfileOperationException("Cannot modify customer group for another user");
                    }
                }

                

                int res = Session.QueryOver<CustomerGroup>().Where(x => x.Name == db.Name && x.GlobalId != db.GlobalId && x.Profile == dbProfile).RowCount();
                if (res > 0)
                {
                    throw new UniqueException("Customer group with the same name is already exist");
                }
                db.Customers.Clear();
                foreach (var customer in customerGroup.Customers)
                {
                    var dbCustomer=Session.QueryOver<Customer>().Fetch(x => x.Groups).Eager.Where(x => x.GlobalId == customer.GlobalId).SingleOrDefault();
                    if (dbProfile != dbCustomer.Profile)
                    {
                        throw new CrossProfileOperationException("Cannot modify customer group for another user");
                    }
                    if(dbCustomer.IsVirtual)
                    {
                        throw new InvalidOperationException("Cannot add virtual customer to the group");
                    }
                    if (db.RestrictedType == CustomerGroupRestrictedType.Partially && dbCustomer.Groups.Where(x => x.RestrictedType != CustomerGroupRestrictedType.None && x.GlobalId != db.GlobalId).Count() > 0)
                    {
                        throw new AlreadyOccupiedException("Customer already belongs to partially or fully restricted group");
                    }

                    if (db.RestrictedType == CustomerGroupRestrictedType.Full && dbCustomer.Groups.Count>0)
                    {
                        throw new AlreadyOccupiedException("Customer already belongs to partially or fully restricted group");
                    }

                    if (dbCustomer.Groups.Where(x => x.RestrictedType == CustomerGroupRestrictedType.Full).Count() > 0)
                    {
                        throw new AlreadyOccupiedException("Customer already belongs to partially or fully restricted group");
                    }
                    db.Customers.Add(dbCustomer);

                    
                }

                db = Session.Merge(db);
                dbProfile.DataInfo.CustomerGroupHash = Guid.NewGuid();
                trans.Commit();
                return db.Map<CustomerGroupDTO>();
            }
        }

        private IQueryOver<CustomerGroup, CustomerGroup> getCustomerGroupsCriterias(IQueryOver<CustomerGroup, CustomerGroup> queryCustomer, CustomerGroupSearchCriteria criteria)
        {
            var dbProfile = Session.Load<Profile>(SecurityInfo.SessionData.Profile.GlobalId);
            queryCustomer = queryCustomer.Where(x => x.Profile == dbProfile);
            if (criteria.MembersCriteria == CustomerGroupMembersCriteria.WithMembersOnly)
            {
                queryCustomer = queryCustomer.WhereRestrictionOn(x => x.Customers).IsNotEmpty;
            }
            else if (criteria.MembersCriteria == CustomerGroupMembersCriteria.WithoutMembersOnly)
            {
                queryCustomer = queryCustomer.WhereRestrictionOn(x => x.Customers).IsEmpty;
            }
            return queryCustomer;
        }

        public PagedResult<CustomerGroupDTO> GetCustomerGroups(CustomerGroupSearchCriteria criteria, PartialRetrievingInfo pageInfo)
        {
            Log.WriteWarning("GetCustomerGroups:Username={0}", SecurityInfo.SessionData.Profile.UserName);

            using (var transactionScope = Session.BeginGetTransaction())
            {

                CustomerGroup _temp = null;
                var idQuery = Session.QueryOver<CustomerGroup>(() => _temp);
                idQuery = (QueryOver<CustomerGroup, CustomerGroup>)getCustomerGroupsCriterias(idQuery, criteria);

                
                var fetchQuery = Session.QueryOver<CustomerGroup>(()=>_temp)
                    .Fetch(x => x.Customers).Eager;
                var listPack = fetchQuery.ToExPagedResults<CustomerGroupDTO, CustomerGroup>(pageInfo, idQuery);
                transactionScope.Commit();
                return listPack;
            }
        }

        public void DeleteCustomerGroup(CustomerGroupDTO customerGroup)
        {
            Log.WriteWarning("DeleteCustomerGroup: Username={0},GlobalId={1}", SecurityInfo.SessionData.Profile.UserName, customerGroup.GlobalId);

            if (!SecurityInfo.Licence.IsInstructor)
            {
                throw new LicenceException("This feature is allowed for Instructor account");
            }

            var session = Session;
            using (var tx = session.BeginSaveTransaction())
            {
                var dbGroup = session.Get<CustomerGroup>(customerGroup.GlobalId);
                if (SecurityInfo.SessionData.Profile.GlobalId != dbGroup.Profile.GlobalId)
                {
                    throw new CrossProfileOperationException("Cannot modify customer group for another user");
                }
                session.Delete(dbGroup);
                dbGroup.Profile.DataInfo.CustomerGroupHash = Guid.NewGuid();
                tx.Commit();
            }
        }
    }
}
