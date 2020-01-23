using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using NHibernate;

namespace BodyArchitect.Service.V2.Services
{
    static class MeasurementsAutomaticUpdater
    {
        public static void Update(ISession session, Profile profile,Customer customer, DateTime updateDate)
        {
            IHasWymiar hasWymiar = (IHasWymiar) customer ?? profile;
            bool automaticUpdate = customer != null
                                       ? customer.Settings.AutomaticUpdateMeasurements
                                       : profile.Settings.AutomaticUpdateMeasurements;
            if (automaticUpdate)
            {
                if (hasWymiar.Wymiary == null)
                {
                    hasWymiar.Wymiary = new Wymiary();
                }
                session.Flush();
                string selectHeightQuery =@"SELECT w.Height FROM TrainingDay td,EntryObject eo,SizeEntry e, Wymiary w WHERE e.Wymiary_id=w.GlobalId AND e.EntryObject_id=eo.GlobalId AND eo.TrainingDay_id=td.GlobalId AND {0} AND td.Profile_id=:ProfileId AND w.Height>0 ORDER BY td.TrainingDate DESC LIMIT 0,1";
                string selectWeightQuery = @"SELECT w.Weight FROM TrainingDay td,EntryObject eo,SizeEntry e, Wymiary w WHERE e.Wymiary_id=w.GlobalId AND e.EntryObject_id=eo.GlobalId AND eo.TrainingDay_id=td.GlobalId AND {0} AND td.Profile_id=:ProfileId AND w.Weight>0 ORDER BY td.TrainingDate DESC LIMIT 0,1";
                string selectKlatkaQuery = @"SELECT w.Klatka FROM TrainingDay td,EntryObject eo,SizeEntry e, Wymiary w WHERE e.Wymiary_id=w.GlobalId AND e.EntryObject_id=eo.GlobalId AND eo.TrainingDay_id=td.GlobalId AND {0} AND td.Profile_id=:ProfileId AND w.Klatka>0 ORDER BY td.TrainingDate DESC LIMIT 0,1";
                string selectLeftBicepsQuery = @"SELECT w.LeftBiceps FROM TrainingDay td,EntryObject eo,SizeEntry e, Wymiary w WHERE e.Wymiary_id=w.GlobalId AND e.EntryObject_id=eo.GlobalId AND eo.TrainingDay_id=td.GlobalId AND {0} AND td.Profile_id=:ProfileId AND w.LeftBiceps>0 ORDER BY td.TrainingDate DESC LIMIT 0,1";
                string selectLeftForearmQuery = @"SELECT w.LeftForearm FROM TrainingDay td,EntryObject eo,SizeEntry e, Wymiary w WHERE e.Wymiary_id=w.GlobalId AND e.EntryObject_id=eo.GlobalId AND eo.TrainingDay_id=td.GlobalId AND {0} AND td.Profile_id=:ProfileId AND w.LeftForearm>0 ORDER BY td.TrainingDate DESC LIMIT 0,1";
                string selectLeftUdoQuery = @"SELECT w.LeftUdo FROM TrainingDay td,EntryObject eo,SizeEntry e, Wymiary w WHERE e.Wymiary_id=w.GlobalId AND e.EntryObject_id=eo.GlobalId AND eo.TrainingDay_id=td.GlobalId AND {0} AND td.Profile_id=:ProfileId AND w.LeftUdo>0 ORDER BY td.TrainingDate DESC LIMIT 0,1";
                string selectPasQuery = @"SELECT w.Pas FROM TrainingDay td,EntryObject eo,SizeEntry e, Wymiary w WHERE e.Wymiary_id=w.GlobalId AND e.EntryObject_id=eo.GlobalId AND eo.TrainingDay_id=td.GlobalId AND {0} AND td.Profile_id=:ProfileId AND w.Pas>0 ORDER BY td.TrainingDate DESC LIMIT 0,1";
                string selectRightBicepsQuery = @"SELECT w.RightBiceps FROM TrainingDay td,EntryObject eo,SizeEntry e, Wymiary w WHERE e.Wymiary_id=w.GlobalId AND e.EntryObject_id=eo.GlobalId AND eo.TrainingDay_id=td.GlobalId AND {0} AND td.Profile_id=:ProfileId AND w.RightBiceps>0 ORDER BY td.TrainingDate DESC LIMIT 0,1";
                string selectRightForearmQuery = @"SELECT w.RightForearm FROM TrainingDay td,EntryObject eo,SizeEntry e, Wymiary w WHERE e.Wymiary_id=w.GlobalId AND e.EntryObject_id=eo.GlobalId AND eo.TrainingDay_id=td.GlobalId AND {0} AND td.Profile_id=:ProfileId AND w.RightForearm>0 ORDER BY td.TrainingDate DESC LIMIT 0,1";
                string selectRightUdoQuery = @"SELECT w.RightUdo FROM TrainingDay td,EntryObject eo,SizeEntry e, Wymiary w WHERE e.Wymiary_id=w.GlobalId AND e.EntryObject_id=eo.GlobalId AND eo.TrainingDay_id=td.GlobalId AND {0} AND td.Profile_id=:ProfileId AND w.RightUdo>0 ORDER BY td.TrainingDate DESC LIMIT 0,1";

                //Wymiary w = null;
                //var res =
                //    session.QueryOver<TrainingDay>().Where(x => x.Profile == profile && x.Customer == customer).OrderBy(
                //        x => x.TrainingDate).Desc.JoinQueryOver(x => x.Objects).JoinQueryOver<SizeEntry>(
                //            x => x.OfType<SizeEntry>().First()).JoinAlias(x => x.Wymiary, () => w).Where(
                //                x => w.Height > 0).Select(x => w.Height).Take(1);
                var query = createQuery(session, selectHeightQuery, profile, customer);
                hasWymiar.Wymiary.Height = getLatestValue(hasWymiar.Wymiary.Height, Convert.ToDecimal(query.UniqueResult()));

                query = createQuery(session, selectWeightQuery, profile, customer);
                hasWymiar.Wymiary.Weight = getLatestValue(hasWymiar.Wymiary.Weight, Convert.ToDecimal(query.UniqueResult()));

                query = createQuery(session, selectKlatkaQuery, profile, customer);
                hasWymiar.Wymiary.Klatka = getLatestValue(hasWymiar.Wymiary.Klatka, Convert.ToDecimal(query.UniqueResult()));

                query = createQuery(session, selectLeftBicepsQuery, profile, customer);
                hasWymiar.Wymiary.LeftBiceps = getLatestValue(hasWymiar.Wymiary.LeftBiceps, Convert.ToDecimal(query.UniqueResult()));

                query = createQuery(session, selectLeftForearmQuery, profile, customer);
                hasWymiar.Wymiary.LeftForearm = getLatestValue(hasWymiar.Wymiary.LeftForearm, Convert.ToDecimal(query.UniqueResult()));

                query = createQuery(session, selectLeftUdoQuery, profile, customer);
                hasWymiar.Wymiary.LeftUdo = getLatestValue(hasWymiar.Wymiary.LeftUdo, Convert.ToDecimal(query.UniqueResult()));

                query = createQuery(session, selectPasQuery, profile, customer);
                hasWymiar.Wymiary.Pas = getLatestValue(hasWymiar.Wymiary.Pas, Convert.ToDecimal(query.UniqueResult()));

                query = createQuery(session, selectRightBicepsQuery, profile, customer);
                hasWymiar.Wymiary.RightBiceps = getLatestValue(hasWymiar.Wymiary.RightBiceps, Convert.ToDecimal(query.UniqueResult()));

                query = createQuery(session, selectRightForearmQuery, profile, customer);
                hasWymiar.Wymiary.RightForearm = getLatestValue(hasWymiar.Wymiary.RightForearm, Convert.ToDecimal(query.UniqueResult()));

                query = createQuery(session, selectRightUdoQuery, profile, customer);
                hasWymiar.Wymiary.RightUdo = getLatestValue(hasWymiar.Wymiary.RightUdo, Convert.ToDecimal(query.UniqueResult()));
                hasWymiar.Wymiary.Time.DateTime = updateDate;
                session.SaveOrUpdate(hasWymiar);
            }
        }

        static decimal getLatestValue(decimal oldValue, decimal newValue)
        {
            if (newValue > 0)
            {
                return newValue;
            }
            return oldValue;
        }

        static ISQLQuery createQuery(ISession session,string strQuery, Profile profile, Customer customer)
        {
            if(customer!=null)
            {
                strQuery = string.Format(strQuery, "td.Customer_id =:CustomerId");
            }
            else
            {
                strQuery = string.Format(strQuery, "td.Customer_id is NULL");
            }
            var query = session.CreateSQLQuery(strQuery);
            query.SetGuid("ProfileId", profile.GlobalId);
            if (customer != null)
            {
                query.SetGuid("CustomerId", customer.GlobalId);
            }
            return query;
        }
    }
}
