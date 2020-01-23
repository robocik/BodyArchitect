using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using BodyArchitect.DataAccess.NHibernate;
using BodyArchitect.Model;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Shared;
using NHibernate;
using Profile = BodyArchitect.Model.Profile;
using ScheduleEntryState = BodyArchitect.Model.ScheduleEntryState;

namespace BodyArchitect.Service.V2.Services
{
    public class ReservationService :ServiceBase
    {
        public ReservationService(ISession session, SecurityInfo securityInfo, ServiceConfiguration configuration)
            : base(session, securityInfo, configuration)
        {
        }

        public ReservationOperationResult ReservationsOperation(ReservationsOperationParam param)
        {
            ReservationOperationResult result = new ReservationOperationResult();
            using (var trans = Session.BeginSaveTransaction())
            {
                var dbEmployee = Session.Load<Profile>(SecurityInfo.SessionData.Profile.GlobalId);
                var dbReservation = param.ReservationId.HasValue ? Session.QueryOver<ScheduleEntryReservation>().Fetch(x=>x.ScheduleEntry).Eager.Where(x => x.GlobalId == param.ReservationId).SingleOrDefault() : null;
                var dbEntry = param.EntryId.HasValue ? Session.Get<ScheduleEntryBase>(param.EntryId) : dbReservation.ScheduleEntry;
                Payment zakup = null;
                if (param.OperationType == ReservationsOperationType.StatusDone)
                {
                    setStatus(dbEntry, ScheduleEntryState.Done);
                }
                else if (param.OperationType == ReservationsOperationType.StatusCancelled)
                {
                    setStatus(dbEntry, ScheduleEntryState.Cancelled);
                }
                else if(param.OperationType==ReservationsOperationType.MakeGroup)
                {
                    makeGroupReservation(param, dbEntry, dbEmployee);
                }
                else
                {
                    var dbCustomer = param.CustomerId.HasValue ? Session.Get<Customer>(param.CustomerId) : dbReservation.Customer;
                    
                    if (dbCustomer.Profile != dbEmployee || (dbEntry.Profile != dbEmployee))
                    {
                        throw new CrossProfileOperationException("Customer or ScheduleEntry belongs to another user");
                    }

                    if (param.OperationType == ReservationsOperationType.Absent)
                    {
                        dbReservation.LeaveDateTime = dbReservation.EnterDateTime = null;
                    }
                    else if (param.OperationType == ReservationsOperationType.Presnet)
                    {
                        if (dbEntry.State != ScheduleEntryState.Done)
                        {
                            throw new InvalidOperationException("Absent can be only set for entries with Done status");
                        }
                        dbReservation.EnterDateTime = dbEntry.StartTime;
                        dbReservation.LeaveDateTime = dbEntry.EndTime;
                    }
                    else if (param.OperationType == ReservationsOperationType.Make)
                    {
                        //get existing reservation for this customer (except virtual customers)
                        if (!dbCustomer.IsVirtual)
                        {
                            dbReservation =
                                dbEntry.Reservations.SingleOrDefault(
                                    x => x.Customer == dbCustomer && x.LeaveDateTime == null);
                        }
                        dbReservation = makeReservation(dbEmployee, dbEntry, dbCustomer, dbReservation);
                        zakup = calculatePayement(dbReservation, false);
                    }
                    else if (param.OperationType == ReservationsOperationType.Undo)
                    {
                        undoReservation(dbReservation, dbEntry);
                    }
                    Session.SaveOrUpdate(dbReservation);
                }
                
                Session.SaveOrUpdate(dbEntry);
                trans.Commit();
                result.Reservation = Mapper.Map<ScheduleEntryReservation, ScheduleEntryReservationDTO>(dbReservation);
                result.ScheduleEntry = dbEntry.Map<ScheduleEntryBaseDTO>();
                result.Payment = Mapper.Map<Payment, PaymentDTO>(zakup);
                //return result;
            }
            return result;
        }

        private static void undoReservation(ScheduleEntryReservation dbReservation, ScheduleEntryBase dbEntry)
        {
            if (dbReservation == null)
            {
                throw new AlreadyOccupiedException("Selected customer isn't on the reservation list");
            }
            if (dbEntry.State != ScheduleEntryState.Planned)
            {
                throw new InvalidOperationException("Cannot undo reservation for entries with other statuses than Planned");
            }
            dbReservation.ScheduleEntry = null;
            dbEntry.Reservations.Remove(dbReservation);

            //Championship championship = dbEntry as Championship;
            //if (championship != null)
            //{
            //    var championshipCustomer=championship.Customers.Where(x => x.Customer.GlobalId == dbReservation.Customer.GlobalId).SingleOrDefault();
            //    if (championshipCustomer != null)
            //    {
            //        championship.Customers.Remove(championshipCustomer);
            //    }
            //    championship.Entries.
            //}
        }

        private void makeGroupReservation(ReservationsOperationParam param, ScheduleEntryBase dbEntry, Profile dbEmployee)
        {
            var dbGroup = Session.Get<CustomerGroup>(param.CustomerId.Value);
            if (dbGroup.Profile != dbEmployee || (dbEntry.Profile != dbEmployee))
            {
                throw new CrossProfileOperationException("CustomerGroup or ScheduleEntry belongs to another user");
            }

            foreach (var dbCustomer in dbGroup.Customers)
            {
                //get existing reservation for this customer. If he is on the list then skip this customer
                var dbReservation =dbEntry.Reservations.SingleOrDefault(x => x.Customer == dbCustomer && x.LeaveDateTime == null);
                if (dbReservation == null)
                {
                    dbReservation = makeReservation(dbEmployee, dbEntry, dbCustomer, dbReservation);
                    calculatePayement(dbReservation, false);
                }
            }
            
        }

        void setStatus(ScheduleEntryBase entry,ScheduleEntryState status)
        {
            Championship championship = entry as Championship;
            if (championship != null)
            {
                championshipStatusDoneImplementation(championship);
            }
            entry.State = status;
        }

        private void championshipStatusDoneImplementation(Championship championship)
        {
            if (championship.Categories.Count == 0)
            {
                throw new ConsistencyException("Championship must have at least one winning category before you set Done status");
            }

            foreach (var reservation in championship.Reservations)
            {
                createChampionshipEntryForCustomer(championship, reservation);
            }
            
        }

        void createChampionshipEntryForCustomer(Championship championship,ScheduleEntryReservation reservation)
        {
            Exercise benchPress = Session.Load<Exercise>(new Guid("ece5dfd7-f995-45ae-bb34-067f26c4f7b4"));
            Exercise deadlift = Session.Load<Exercise>(new Guid("505988e1-5663-41f1-aa1a-9b92ea584263"));
            Exercise squad = Session.Load<Exercise>(new Guid("3e06a130-b811-4e45-9285-f087403615bf"));

            var championshipCustomer = new ChampionshipCustomer();
            championshipCustomer.Customer = reservation.Customer;
            championship.Customers.Add(championshipCustomer);

            ChampionshipEntry entryChamp = new ChampionshipEntry();
            entryChamp.Customer = championshipCustomer;
            entryChamp.Exercise = benchPress;
            championship.Entries.Add(entryChamp);

            if (championship.ChampionshipType == ChampionshipType.Trojboj)
            {
                entryChamp = new ChampionshipEntry();
                entryChamp.Customer = championshipCustomer;
                entryChamp.Exercise = deadlift;
                championship.Entries.Add(entryChamp);

                entryChamp = new ChampionshipEntry();
                entryChamp.Customer = championshipCustomer;
                entryChamp.Exercise = squad;
                championship.Entries.Add(entryChamp);
            }
        }

        private ScheduleEntryReservation makeReservation(Profile dbProfile, ScheduleEntryBase dbEntry, Customer dbCustomer, ScheduleEntryReservation res)
        {
            //except anonymous cusomers
            if (res != null && !dbCustomer.IsVirtual)
            {
                throw new AlreadyOccupiedException("Selected customer is already in the reservation list");
            }
            Championship championship = dbEntry as Championship;
            if (championship != null && dbCustomer.IsVirtual)
            {
                throw new InvalidOperationException("Cannot make reservation to championship for virtual customer");
            }
            ScheduleEntryReservation reservation = null;

            if (dbEntry.State != ScheduleEntryState.Planned)
            {
                throw new InvalidOperationException("Cannot make reservations for entries with Done state");
            }
            if ( !dbCustomer.IsVirtual)
            {
                //check if this customer has reservation for another acitivity at the same time
                int count = Session.QueryOver<ScheduleEntry>().JoinAlias(x => x.Reservations, () => reservation).Where(
                    x => x.GlobalId != dbEntry.GlobalId && (x.StartTime <= dbEntry.StartTime && x.EndTime > dbEntry.StartTime ||
                                                x.StartTime < dbEntry.EndTime && x.EndTime >= dbEntry.EndTime) &&
                         x.Profile == dbProfile && reservation.Customer == dbCustomer).RowCount();

                if (count > 0)
                {
                    throw new AlreadyOccupiedException("Selected customer is already in the reservation list for another activity");
                }
            }
            reservation = new ScheduleEntryReservation();
            reservation.DateTime = Configuration.TimerService.UtcNow;
            reservation.Customer = dbCustomer;
            reservation.Profile = dbProfile;
            //if (dbEntry != null)
            //{
                reservation.ScheduleEntry = dbEntry;
                dbEntry.Reservations.Add(reservation);
            //}
            //else
            //{
            //    reservation.Room = dbRoom;
            //    reservation.Usluga = dbRoom.FreeEnterUsluga;
            //}
            ScheduleEntry activityEntry = dbEntry as ScheduleEntry;
            Championship championEntry = dbEntry as Championship;
            if (activityEntry != null)
            {

                reservation.Name = string.Format("{0}:{1}", activityEntry.Activity.Name, dbEntry.StartTime);
            }
            else
            {
                //championship entry
                //TODO:Translate
                reservation.Name = championEntry.ChampionshipType.ToString();
            }

            //Championship championship = dbEntry as Championship;
            //if (championship != null)
            //{
            //    createChampionshipEntryForCustomer(championship,reservation);
            //}
            res = reservation;
            Session.Save(res);
            return res;
        }

        Payment calculatePayement(ScheduleEntryReservation reservation, bool forLeave)
        {
            Payment zakup = null;
            if (reservation.Payment == null)
            {
                reservation.Price = reservation.ScheduleEntry.Price;
                if (reservation.Price > 0)
                {
                    zakup = new Payment();
                    zakup.Product = reservation;
                    zakup.DateTime = Configuration.TimerService.UtcNow;
                    zakup.Count = 1;
                }
            }

            //if (forLeave)
            //{

            //    //sprawdz czy zaplacono karnetem gdzie byla informacja o dopłacie za przekroczenie czasu. Jesli trzeba to nalezy tez odliczyc czas pobytu z pozostałęgo czasu na karnecie
            //    NHibernateUtil.Initialize(reservation.Payement);
            //    var wejscieKarnet = reservation.Payement.GetRealObject<ZakupBase>() as WejscieNaKarnet;
            //    var time = reservation.LeaveDateTime.Value - reservation.EnterDateTime.Value;
            //    TaryfaPrice taryfaPrice = getCurrentTaryfaPrice(reservation, wejscieKarnet);

            //    if (wejscieKarnet != null && wejscieKarnet.Karnet.PozostalyCzas != null)
            //    {//karnet przechowuje pozostały czas do wykorzystania dlatego trzeba zakutalizować tą liczbę
            //        wejscieKarnet.Karnet.PozostalyCzas -= time;
            //        if (wejscieKarnet.Karnet.PozostalyCzas <= TimeSpan.Zero)
            //        {
            //            wejscieKarnet.Karnet.State = KarnetState.Expired;
            //        }
            //        if (wejscieKarnet.Karnet.PozostalyCzas.Value.TotalMinutes < 0)
            //        {//musimy obliczyc doplate bo przekroczono czas pobytu a na karnecie tyle nie ma
            //            decimal? doplata = taryfaPrice.Doplata;
            //            if (doplata == null)
            //            {//doplata nie została ustawiona także nalicz kwotę dopłaty z ceny usługi
            //                //tutaj już nie podajemy karnetu żeby cena została pobrana od usługi tylko
            //                doplata = calculateDoplataFromUslugaPrice(reservation, wejscieKarnet);
            //            }
            //            zakup = createDoplata(reservation, doplata.Value, wejscieKarnet.Karnet.PozostalyCzas.Value.Duration(), zakup);
            //            //wyzeruj pozostaly czas zeby nastepnym razem nie mozna bylo wejsc na ten karnet
            //            wejscieKarnet.Karnet.PozostalyCzas = TimeSpan.Zero;
            //        }
            //    }
            //    else if (reservation.Usluga.DoplataStartAfter != null)
            //    {
            //        //calculate a price
            //        //TODO:Check. For usluga can have default taryfa price not defined. Is this good?
            //        if (taryfaPrice != null && taryfaPrice.Doplata.HasValue && taryfaPrice.Doplata.Value > 0)
            //        {
            //            var diff = (time - reservation.Usluga.CzasTrwania) - reservation.Usluga.DoplataStartAfter.Value;
            //            zakup = createDoplata(reservation, taryfaPrice.Doplata.Value, diff, zakup);
            //        }
            //    }
            //}

            return zakup;
        }
    }
}
