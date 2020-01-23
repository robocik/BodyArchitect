using System;
using System.Collections.Generic;
using AutoMapper;
using BodyArchitect.DataAccess.NHibernate;
using BodyArchitect.Logger;
using BodyArchitect.Model;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Shared;
using NHibernate;
using NHibernate.Transform;
using ExerciseType = BodyArchitect.Service.V2.Model.ExerciseType;
using Gender = BodyArchitect.Model.Gender;
using Profile = BodyArchitect.Model.Profile;
using PublishStatus = BodyArchitect.Service.V2.Model.PublishStatus;
using ReminderRepetitions = BodyArchitect.Model.ReminderRepetitions;
using ReminderType = BodyArchitect.Model.ReminderType;

namespace BodyArchitect.Service.V2.Services
{
    public class CustomerService : ServiceBase
    {
        public CustomerService(ISession session, SecurityInfo securityInfo, ServiceConfiguration configuration) : base(session, securityInfo, configuration)
        {
        }

        public PagedResult<CustomerDTO> GetCustomers(CustomerSearchCriteria customerSearchCriteria, PartialRetrievingInfo pageInfo)
        {
            Log.WriteWarning("GetCustomers:Username={0}", SecurityInfo.SessionData.Profile.UserName);

            using (var transactionScope = Session.BeginGetTransaction())
            {
                var dbProfile = Session.Load<Profile>(SecurityInfo.SessionData.Profile.GlobalId);
                var queryCustomer = Session.QueryOver<Customer>().Where(x => x.Profile == dbProfile).
                    Fetch(x => x.ConnectedAccount).Eager.
                    Fetch(x => x.Wymiary).Eager.
                    Fetch(x => x.Settings).Eager.
                    Fetch(x => x.Address).Eager;

                if(customerSearchCriteria.Gender.HasValue)
                {
                    queryCustomer = queryCustomer.Where(x => x.Gender == (Gender) customerSearchCriteria.Gender.Value);
                }
                if(customerSearchCriteria.CustomerVirtualCriteria!=CustomerVirtualCriteria.All)
                {
                    queryCustomer = queryCustomer.Where(x => x.IsVirtual == (customerSearchCriteria.CustomerVirtualCriteria == CustomerVirtualCriteria.VirtualOnly));
                }
                //queryCustomer = queryCustomer.TransformUsing(Transformers.DistinctRootEntity);
                var listPack = queryCustomer.ToPagedResults<CustomerDTO, Customer>(pageInfo);
                transactionScope.Commit();
                return listPack;
            }
        }

        public CustomerDTO SaveCustomer(CustomerDTO customerDto)
        {
            Log.WriteWarning("SaveCustomer:Username={0},GlobalId={1}", SecurityInfo.SessionData.Profile.UserName, customerDto.GlobalId);

            if (!customerDto.IsNew && !SecurityInfo.Licence.IsInstructor)
            {
                throw new LicenceException("This feature is allowed for Instructor account");
            }

            var db = customerDto.Map<Customer>();
            using (var trans = Session.BeginSaveTransaction())
            {
                Profile dbProfile = Session.Load<Profile>(SecurityInfo.SessionData.Profile.GlobalId);
                Picture oldPicture = null;
                Wymiary oldWymiary = null;
                Customer oldCustomer = null;
                Address oldAddress = null;
                CustomerSettings oldSettings = null;

                bool oldIsVirtual;
                //if (!customerDto.IsNew)
                //{
                //    db = Session.Get<Customer>(customerDto.GlobalId);
                //    oldPicture = db.Picture;
                //    oldWymiary = db.Wymiary;
                //    oldSettings = db.Settings;
                //    oldIsVirtual = db.IsVirtual;
                //    Mapper.Map(customerDto, db);

                //    if(oldIsVirtual!=db.IsVirtual)
                //    {
                //        throw new InvalidOperationException("Cannot change IsVirtual for existing customer");
                //    }
                //}
                //else
                //{
                //    db = customerDto.Map<Customer>();
                //    db.CreationDate = Configuration.TimerService.UtcNow;
                //    db.Settings = new CustomerSettings();
                //    db.Profile = dbProfile;
                //}

                if (db.IsNew)
                {
                    db.CreationDate = Configuration.TimerService.UtcNow;
                    db.Settings = new CustomerSettings();
                }
                else
                {
                    oldCustomer = Session.QueryOver<Customer>().Where(x => x.GlobalId == db.GlobalId)
                        .Fetch(x=>x.Address).Eager
                        .Fetch(x => x.Wymiary).Eager
                        .Fetch(x => x.Settings).Eager
                        .Fetch(x => x.Picture).Eager
                        .SingleOrDefault();
                    if (oldCustomer != null)
                    {
                        if (dbProfile != oldCustomer.Profile)
                        {
                            throw new CrossProfileOperationException("Cannot modify Customer for another user");
                        }

                        oldPicture = oldCustomer.Picture;
                        oldWymiary = oldCustomer.Wymiary;
                        oldAddress = oldCustomer.Address;
                        oldSettings = oldCustomer.Settings;
                        oldIsVirtual = oldCustomer.IsVirtual;
                        if (oldIsVirtual != db.IsVirtual)
                        {
                            throw new InvalidOperationException("Cannot change IsVirtual for existing customer");
                        }
                    }
                }


                db.Profile = dbProfile;

                if (customerDto.ConnectedAccount != null)
                {
                    Guid connectedAccountId = customerDto.ConnectedAccount.GlobalId;
                    var connectedAccountExists =
                        Session.QueryOver<Customer>().Where(x => x.Profile == dbProfile && x.GlobalId != db.GlobalId &&
                                                                 x.ConnectedAccount.GlobalId == connectedAccountId
                                                                 ).RowCount();
                    if (connectedAccountExists > 0)
                    {
                        throw new AlreadyOccupiedException("Specified ConnectedAccount is already used");
                    }
                    db.ConnectedAccount = Session.Get<Profile>(connectedAccountId);
                }
                else
                {
                    db.ConnectedAccount = null;
                }

                if (db.Wymiary != null && db.Wymiary.IsEmpty)
                {
                    Log.WriteVerbose("Wymiary is empty.");
                    if (!db.Wymiary.IsNew && (oldWymiary == null || db.Wymiary.GlobalId != oldWymiary.GlobalId))
                    {
                        Log.WriteInfo("Delete wymiary from db");
                        Session.Delete(db.Wymiary);
                    }
                    db.Wymiary = null;
                }

                if (oldWymiary != null && (db.Wymiary == null || oldWymiary.GlobalId != db.Wymiary.GlobalId))
                {
                    Session.Delete(oldWymiary);
                }

                if (db.Settings == null)
                {
                    throw new ArgumentNullException("Settings cannot be null");
                }


                if (db.Address != null && db.Address.IsEmpty)
                {
                    Log.WriteVerbose("Address is empty.");
                    if (!db.Address.IsNew && (oldAddress==null || db.Address.GlobalId != oldAddress.GlobalId))
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

                if (oldSettings != null && oldSettings.GlobalId != db.Settings.GlobalId)
                {
                    Session.Delete(oldSettings);
                }

                if (db.Birthday != null)
                {
                    var reminderService = new ReminderService(Session, SecurityInfo, Configuration);
                    reminderService.PrepareReminder(dbProfile, customerDto, db, oldCustomer, db.Birthday.Value.Date,
                                                    ReminderType.Birthday,ReminderRepetitions.EveryYear);
                }
                else if (oldCustomer!=null && oldCustomer.Reminder != null)
                {
                    Session.Delete(oldCustomer.Reminder);
                    oldCustomer.Reminder = null;
                    dbProfile.DataInfo.ReminderHash = Guid.NewGuid();
                }

                //remove old picture;
                PictureService pictureService = new PictureService(Session, SecurityInfo, Configuration);
                pictureService.DeletePictureLogic(oldPicture, customerDto.Picture);


                db = Session.Merge(db);
                dbProfile.DataInfo.CustomerHash = Guid.NewGuid();
                if(db.Reminder!=null)
                {
                    db.Reminder.ConnectedObject = string.Format("CustomerDTO:{0}", db.GlobalId);
                    Session.Update(db.Reminder);
                    //TODO:Update datainfo for reminder
                }

                if (db.Settings.AutomaticUpdateMeasurements)
                {
                    MeasurementsAutomaticUpdater.Update(Session, dbProfile, db, Configuration.TimerService.UtcNow);
                }

                trans.Commit();
                return db.Map<CustomerDTO>();
            }
        }

        public void DeleteCustomer(CustomerDTO customerDto)
        {
            Log.WriteWarning("DeleteCustomer: Username={0},GlobalId={1}", SecurityInfo.SessionData.Profile.UserName, customerDto.GlobalId);
            if (!SecurityInfo.Licence.IsInstructor)
            {
                throw new LicenceException("This feature is allowed for Instructor account");
            }
            var session = Session;
            using (var tx = session.BeginSaveTransaction())
            {
                var dbCustomer = session.Get<Customer>(customerDto.GlobalId);
                if (SecurityInfo.SessionData.Profile.GlobalId != dbCustomer.Profile.GlobalId)
                {
                    throw new CrossProfileOperationException("Cannot delete Customer for another user");
                }
                if (dbCustomer.Reminder != null)
                {
                    dbCustomer.Profile.DataInfo.ReminderHash = Guid.NewGuid();
                }
                
                

                Customer cust = null;
                var dbGroups = Session.QueryOver<CustomerGroup>().JoinAlias(x => x.Customers,()=>cust).Where(x => cust.GlobalId == dbCustomer.GlobalId).List();
                foreach (var customerGroup in dbGroups)
                {
                    customerGroup.Customers.Remove(dbCustomer);
                    session.Update(customerGroup);
                }

                session.Delete("FROM TrainingDay WHERE Customer=?", dbCustomer.GlobalId, NHibernateUtil.Guid);
                session.Delete("FROM MyTraining WHERE Customer=?", dbCustomer.GlobalId, NHibernateUtil.Guid);
                session.Delete("FROM ExerciseProfileData WHERE Customer=?", dbCustomer.GlobalId, NHibernateUtil.Guid);

                session.Delete(dbCustomer);
                dbCustomer.Profile.DataInfo.CustomerHash = Guid.NewGuid();

                tx.Commit();
            }
        }
    }
}
