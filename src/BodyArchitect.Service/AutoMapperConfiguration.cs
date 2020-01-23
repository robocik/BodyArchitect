using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using BodyArchitect.Service.Model;

namespace BodyArchitect.Service
{
    public class AutoMapperConfiguration : AutoMapper.Profile
    {
        /*public static Helpers h = new Helpers();

        public override string ProfileName
        {
            get
            {
                return "FirstVersion";
            }
        }

        public static void Configure()
        {
            Mapper.Initialize(x =>
            {
                x.ConstructServicesUsing(createServices);
            });
            Mapper.CreateMap<Token, V2.Model.Token>();
            Mapper.CreateMap<V2.Model.Token, Token>();

            Mapper.CreateMap<V2.Model.ExerciseSearchCriteria, ExerciseSearchCriteria>()
                .ForMember(a => a.UserId, b => b.Ignore())
                .AfterMap(delegate(V2.Model.ExerciseSearchCriteria s, ExerciseSearchCriteria d)
                {
                    int? tempId = null;
                    if ((tempId = h.GetIntFromGuid(s.UserId)) == null)
                        tempId = h.SetIntFromGuid((Guid)s.UserId, s);
                    if (tempId != null)
                        d.UserId = tempId;
                    else
                        throw new ArgumentException("Id not assigned to guid");
                });
            Mapper.CreateMap<ExerciseSearchCriteria, V2.Model.ExerciseSearchCriteria>()
                .ForMember(a => a.UserId, b => b.Ignore())
                .AfterMap(delegate(ExerciseSearchCriteria s, V2.Model.ExerciseSearchCriteria d)
                {
                    Guid? guid = null;
                    if ((guid = h.GetGuidFromInt(s.UserId, s)) == null)
                        guid = null;
                });

            Mapper.CreateMap<PartialRetrievingInfo, V2.Model.PartialRetrievingInfo>();
            Mapper.CreateMap<V2.Model.PartialRetrievingInfo, PartialRetrievingInfo>();

            
            //Mapper.CreateMap<MyTrainingDTO, V2.Model.MyTrainingDTO>();
            //Mapper.CreateMap<V2.Model.MyTrainingDTO, MyTrainingDTO>();

            //Mapper.CreateMap<ClientInformation, V2.Model.ClientInformation>();
            Mapper.CreateMap<V2.Model.SessionData, SessionData>();

            Mapper.CreateMap<V2.Model.ExerciseDTO, ExerciseDTO>();
            Mapper.CreateMap<V2.Model.MechanicsType, MechanicsType>();
            Mapper.CreateMap<V2.Model.PublishStatus, PublishStatus>();
            Mapper.CreateMap<V2.Model.ExerciseDifficult, ExerciseDifficult>();
            Mapper.CreateMap<V2.Model.ExerciseForceType, ExerciseForceType>();
            Mapper.CreateMap<V2.Model.IRatingable, IRatingable>();
            Mapper.CreateMap<V2.Model.IBelongToUser, IBelongToUser>();
            Mapper.CreateMap<V2.Model.UserDTO, UserDTO>()
                .ForMember(a => a.Id, b => b.Ignore())
                .AfterMap(delegate(V2.Model.UserDTO s, UserDTO d)
                {
                    int? tempId = null;
                    if ((tempId = h.GetIntFromGuid(s.GlobalId)) == null)
                        tempId = h.SetIntFromGuid(s);
                    if (tempId != null)
                        d.Id = (int)tempId;
                    else
                        throw new ArgumentException("Id not assigned to guid");
                });

            Mapper.CreateMap<V2.Model.Gender, Gender>();
            Mapper.CreateMap<V2.Model.ProfileSettingsDTO, ProfileSettingsDTO>()
                .ForMember(a => a.Id, b => b.Ignore())
                .AfterMap(delegate(V2.Model.ProfileSettingsDTO s, ProfileSettingsDTO d)
                {
                    int? tempId = null;
                    if ((tempId = h.GetIntFromGuid(s.GlobalId)) == null)
                        tempId = h.SetIntFromGuid(s);
                    if (tempId != null)
                        d.Id = (int)tempId;
                    else
                        throw new ArgumentException("Id not assigned to guid");
                });
            Mapper.CreateMap<V2.Model.BAGlobalObject, BAObject>()
                .ForMember(a => a.Id, b => b.Ignore())
                .AfterMap(delegate(V2.Model.BAGlobalObject s, BAObject d)
                {
                    int? tempId = null;
                    if ((tempId = h.GetIntFromGuid(s.GlobalId)) == null)
                        tempId = h.SetIntFromGuid(s);
                    if (tempId != null)
                        d.Id = (int)tempId;
                    else
                        throw new ArgumentException("Id not assigned to guid");
                });
            Mapper.CreateMap<V2.Model.IHasPicture, IHasPicture>();
            Mapper.CreateMap<V2.Model.PictureInfoDTO, PictureInfoDTO>();
            Mapper.CreateMap<V2.Model.IToken, IToken>();
            Mapper.CreateMap<V2.Model.ProfilePrivacyDTO, ProfilePrivacyDTO>();
            Mapper.CreateMap<V2.Model.IRetrievedDateTime, IRetrievedDateTime>();

            Mapper.CreateMap<V2.Model.PagedResult<V2.Model.ExerciseDTO>, PagedResult<ExerciseDTO>>();

            
        }

        private static object createServices(Type t)
        {
            if (t == typeof(PagedResult<ExerciseDTO>))
            {
                return PagedResult<ExerciseDTO>.CreateEmpty();
            }
            return null;
        }*/
    }
}
