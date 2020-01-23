using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using BodyArchitect.DataAccess.NHibernate;
using BodyArchitect.Model;

namespace BodyArchitect.Service
{
    public class Helpers
    {
        public int? GetIntFromGuid(Guid guid)
        {
            ISession session = NHibernateContext.Current().Session;
            int? intId;
            using (var tx = session.BeginTransaction())
            {
                //intId = (int?)session.CreateSQLQuery("SELECT i.intid FROM idconvertions i WHERE i.guidid = :guid")
                //    .SetString("guid", guid.ToString()).UniqueResult();
                var result = session.QueryOver<IdConvertion>().Where(b => b.GuidId == guid).SingleOrDefault();
                tx.Commit();
                if (result != null)
                    intId = result.IntId;
                else
                    intId = null;
            }
            return intId;
        }

        public int? GetIntFromGuid(Guid? guid)
        {
            if (guid == null)
                return null;
            else
                return GetIntFromGuid((Guid)guid);
        }

        public int? SetIntFromGuid(BodyArchitect.Service.V2.Model.BAGlobalObject obj)
        {
            ISession session = NHibernateContext.Current().Session;

            int? intId = null;
            using (var tx = session.BeginTransaction())
            {
                string type = obj.GetType().Name;
                //var count = session.CreateSQLQuery("SELECT COUNT(*) FROM idconvertion i WHERE i.type = :type").SetString("type", type).UniqueResult();
                //var result = session.CreateQuery("INSERT INTO idconvertion VALUES (:count, :type, :guid)")
                //    .SetInt32("count", (int)count+1).SetGuid("guid", obj.GlobalId).SetString("type", type).UniqueResult();
                var count = session.QueryOver<IdConvertion>().Where(a => a.Type == obj.GetType().Name).RowCount();
                IdConvertion temp = new IdConvertion();
                temp.GuidId = obj.GlobalId;
                temp.IntId = ++count;
                temp.Type = obj.GetType().Name;
                session.Save(temp);
                //session.Save(
                tx.Commit();

                intId = temp.IntId;
            }
            return intId;
        }

        public Guid? GetGuidFromInt(BodyArchitect.Service.Model.BAObject obj)
        {
            ISession session = NHibernateContext.Current().Session;
            Guid? guid;
            using (var tx = session.BeginTransaction())
            {
                var result = session.QueryOver<IdConvertion>().Where(b => b.IntId == obj.Id).And(c => c.Type == obj.GetType().Name).SingleOrDefault();
                tx.Commit();
                if (result != null)
                    guid = result.GuidId;
                else
                    guid = null;
            }
            return guid;
        }

        public Guid? GetGuidFromInt(BodyArchitect.Service.Model.GetProfileInformationCriteria obj)
        {
            ISession session = NHibernateContext.Current().Session;
            Guid? guid;
            using (var tx = session.BeginTransaction())
            {
                var result = session.QueryOver<IdConvertion>().Where(b => b.IntId == obj.UserId).And(c => c.Type == obj.GetType().Name).SingleOrDefault();
                tx.Commit();
                if (result != null)
                    guid = result.GuidId;
                else
                    guid = null;
            }
            return guid;
        }

        public Guid? SetGuidFromInt(BodyArchitect.Service.Model.GetProfileInformationCriteria obj)
        {
            ISession session = NHibernateContext.Current().Session;

            Guid? guid = null;
            using (var tx = session.BeginTransaction())
            {
                string type = obj.GetType().Name;
                //var count = session.CreateSQLQuery("SELECT COUNT(*) FROM idconvertion i WHERE i.type = :type").SetString("type", type).UniqueResult();
                //var result = session.CreateQuery("INSERT INTO idconvertion VALUES (:count, :type, :guid)")
                //    .SetInt32("count", (int)count+1).SetGuid("guid", obj.GlobalId).SetString("type", type).UniqueResult();
                //var count = session.QueryOver<IdConvertion>().Where(a => a.Type == obj.GetType().Name).RowCount();
                IdConvertion temp = new IdConvertion();
                temp.GuidId = Guid.NewGuid();
                if (obj.UserId != null)
                    temp.IntId = (int)obj.UserId;
                else
                {
                    var count = session.QueryOver<IdConvertion>().Where(a => a.Type == obj.GetType().Name).RowCount();
                    temp.IntId = ++count;
                }
                temp.Type = obj.GetType().Name;
                session.Save(temp);
                tx.Commit();

                guid = temp.GuidId;
            }
            return guid;
        }

        public int? SetIntFromGuid(Guid guid, object obj)
        {
            ISession session = NHibernateContext.Current().Session;

            int? intId = null;
            using (var tx = session.BeginTransaction())
            {
                string type = obj.GetType().Name;
                //var count = session.CreateSQLQuery("SELECT COUNT(*) FROM idconvertion i WHERE i.type = :type").SetString("type", type).UniqueResult();
                //var result = session.CreateQuery("INSERT INTO idconvertion VALUES (:count, :type, :guid)")
                //    .SetInt32("count", (int)count+1).SetGuid("guid", obj.GlobalId).SetString("type", type).UniqueResult();
                var count = session.QueryOver<IdConvertion>().Where(a => a.Type == obj.GetType().Name).RowCount();
                IdConvertion temp = new IdConvertion();
                temp.GuidId = guid;
                temp.IntId = ++count;
                temp.Type = obj.GetType().Name;
                session.Save(temp);
                tx.Commit();

                intId = temp.IntId;
            }
            return intId;
        }

        public Guid? GetGuidFromInt(int? id, object obj)
        {
            ISession session = NHibernateContext.Current().Session;
            Guid? guid;
            using (var tx = session.BeginTransaction())
            {
                var result = session.QueryOver<IdConvertion>().Where(b => b.IntId == id).And(c => c.Type == obj.GetType().Name).SingleOrDefault();
                tx.Commit();
                if (result != null)
                    guid = result.GuidId;
                else
                    guid = null;
            }
            return guid;
        }

        public Guid? SetGuidFromInt(int? id, object obj)
        {
            ISession session = NHibernateContext.Current().Session;

            Guid? guid = null;
            using (var tx = session.BeginTransaction())
            {
                string type = obj.GetType().Name;
                //var count = session.CreateSQLQuery("SELECT COUNT(*) FROM idconvertion i WHERE i.type = :type").SetString("type", type).UniqueResult();
                //var result = session.CreateQuery("INSERT INTO idconvertion VALUES (:count, :type, :guid)")
                //    .SetInt32("count", (int)count+1).SetGuid("guid", obj.GlobalId).SetString("type", type).UniqueResult();
                //var count = session.QueryOver<IdConvertion>().Where(a => a.Type == obj.GetType().Name).RowCount();
                IdConvertion temp = new IdConvertion();
                temp.GuidId = Guid.NewGuid();
                if (id != null)
                    temp.IntId = (int)id;
                 else
                {
                    var count = session.QueryOver<IdConvertion>().Where(a => a.Type == obj.GetType().Name).RowCount();
                    temp.IntId = ++count;
                }
                temp.Type = obj.GetType().Name;
                session.Save(temp);
                tx.Commit();

                guid = temp.GuidId;
            }
            return guid;
        }
    }
}
