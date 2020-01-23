using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using BodyArchitect.Model;
using NHibernate;

namespace BodyArchitect.Service.V2
{
    public class LoadingGuidEntityResolver<TEntity> : ValueResolver<Guid?, TEntity>
      where TEntity : FMGlobalObject
    {
        private readonly ISession _session;

        public LoadingGuidEntityResolver(ISession session)
        {
            _session = session;
        }

        protected override TEntity ResolveCore(Guid? source)
        {
            if (source.HasValue && source!=Guid.Empty)
            {
                return _session.Load<TEntity>(source);
            }
            return null;
        }
    }
}
