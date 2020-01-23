using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using NHibernate;

namespace BodyArchitect.DataAccess.NHibernate
{
    public class NHibernateContextExtension : IExtension<InstanceContext>
    {
        public NHibernateContextExtension(ISession session)
        {
            Session = session;
        }

        public ISession Session { get; private set; }

        public void Attach(InstanceContext owner)
        { }

        public void Detach(InstanceContext owner)
        { }
    }
}
