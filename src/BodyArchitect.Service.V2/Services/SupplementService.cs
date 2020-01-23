using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using BodyArchitect.DataAccess.NHibernate;
using BodyArchitect.Logger;
using BodyArchitect.Model;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using NHibernate;
using NHibernate.Criterion.Lambda;
using NHibernate.Linq;

namespace BodyArchitect.Service.V2.Services
{
    class SupplementService: ServiceBase
    {
        public SupplementService(ISession session, SecurityInfo securityInfo, ServiceConfiguration configuration)
            : base(session, securityInfo, configuration)
        {
        }

        public PagedResult<SuplementDTO> GetSuplements(GetSupplementsParam param, PartialRetrievingInfo retrievingInfo)
        {
            Log.WriteWarning("GetSuplements:Username={0}", SecurityInfo.SessionData.Profile.UserName);
            var session = Session;
            var profileResult = session.QueryOver<Suplement>();

            if(param.ProductCriteria==SupplementProductCriteria.OnlyProducts)
            {
                profileResult = profileResult.Where(x => x.IsProduct);
            }
            else if (param.ProductCriteria == SupplementProductCriteria.OnlyGeneral)
            {
                profileResult = profileResult.Where(x => !x.IsProduct);
            }

            if (param.LegalCriteria == CanBeIllegalCriteria.OnlyIllegal)
            {
                profileResult = profileResult.Where(x => x.CanBeIllegal);
            }
            else if (param.LegalCriteria == CanBeIllegalCriteria.OnlyLegal)
            {
                profileResult = profileResult.Where(x => !x.CanBeIllegal);
            }
            profileResult = profileResult.ApplySorting(param.SortOrder, param.SortAscending);

            var res1 = (from rv in session.Query<RatingUserValue>()
                        from tp in session.Query<Suplement>()
                        where tp.GlobalId == rv.RatedObjectId &&
                            rv.ProfileId == SecurityInfo.SessionData.Profile.GlobalId
                        select rv).ToDictionary(t => t.RatedObjectId);
            
            return profileResult.ToPagedResults<SuplementDTO, Suplement>(retrievingInfo,null,
                          delegate(IEnumerable<Suplement> list)
                          {
                              var output = new List<SuplementDTO>();
                              foreach (var planDto in list)
                              {

                                  var tmp = planDto.Map<SuplementDTO>();
                                  if (res1.ContainsKey(planDto.GlobalId))
                                  {
                                      tmp.UserRating = res1[planDto.GlobalId].Rating;
                                      tmp.UserShortComment = res1[planDto.GlobalId].ShortComment;
                                  }
                                  output.Add(tmp);
                              }
                              return output.ToArray();
                          });
        }

        public SuplementDTO SaveSuplement( SuplementDTO suplement)
        {
            var dbExercise = Mapper.Map<SuplementDTO, Suplement>(suplement);
            var session = Session;
            using (var tx = session.BeginSaveTransaction())
            {
                Suplement dbSuplement;
                if(dbExercise.IsNew)
                {
                    dbExercise.CreationDate = Configuration.TimerService.UtcNow;
                    dbSuplement = dbExercise;
                }
                else
                {
                    dbSuplement = session.Get<Suplement>(suplement.GlobalId);
                }

                if (dbSuplement.Profile == null)
                {
                    throw new CrossProfileOperationException("Cannot change default suplement");
                }
                if (dbSuplement.Profile.GlobalId != suplement.ProfileId || suplement.ProfileId != SecurityInfo.SessionData.Profile.GlobalId)
                {
                    throw new CrossProfileOperationException("Cannot  change an suplement for another profile");
                }

                dbExercise = (Suplement)session.Merge(dbExercise);
                tx.Commit();
            }
            return Mapper.Map<Suplement, SuplementDTO>(dbExercise);
        }
    }
}
