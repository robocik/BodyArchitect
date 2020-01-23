using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using AutoMapper;
using BodyArchitect.DataAccess.NHibernate;
using BodyArchitect.Model;
using BodyArchitect.Service.Admin.Objects;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Services;
using NHibernate.Linq;
using MessagePriority = BodyArchitect.Service.V2.Model.MessagePriority;
using Profile = BodyArchitect.Model.Profile;

namespace BodyArchitect.Service.Admin
{
    public enum SendMessageMode
    {
        All,
        SelectedCountries,
        ExceptSelectedCountries
    }

    [NHibernateContext]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, IncludeExceptionDetailInFaults = true)]
    public class AdminService : IAdminService
    {
        static AdminService()
        {
            ObjectsConverter.Configure();
        }

        public void SendMessage(string topic, string message, SendMessageMode mode, List<int> countriesId)
        {
            var session = NHibernateContext.Current().Session;

            using (var trans = session.BeginTransaction())
            {
                //get admin
                var admin = session.QueryOver<Profile>().Where(x=>x.UserName=="Admin").SingleOrDefault();
                IList<Profile> profiles = null;
                if (mode == SendMessageMode.All)
                {
                    profiles = session.QueryOver<Profile>().Where(x => !x.IsDeleted && x.GlobalId != admin.GlobalId).List();
                }
                else if (mode == SendMessageMode.SelectedCountries)
                {
                    profiles = session.QueryOver<Profile>().Where(x => !x.IsDeleted && x.GlobalId != admin.GlobalId).WhereRestrictionOn(x => x.CountryId).IsIn(countriesId).List();
                }
                else
                {
                    profiles = session.QueryOver<Profile>().Where(x => !x.IsDeleted && x.GlobalId != admin.GlobalId).WhereRestrictionOn(x => x.CountryId).Not.IsIn(countriesId).List();
                }
                foreach (var profile in profiles)
                {
                    var msg = new Message();
                    msg.Content = message;
                    msg.Topic = topic;
                    msg.Sender = admin;
                    msg.Receiver = profile;
                    msg.CreatedDate = DateTime.UtcNow;
                    msg.Priority = (Model.MessagePriority) MessagePriority.System;
                    session.Save(msg);
                }

                trans.Commit();
            }
        }

        public IList<PictureInfoDTO> DeleteUnusedImages(DeleteOldProfilesParam param)
        {
            var profilesWithImages = NHibernateContext.Current().Session.QueryOver<Profile>().Where(x => x.Picture != null).List();
            var customersWithImages = NHibernateContext.Current().Session.QueryOver<Customer>().Where(x => x.Picture != null).List();
            List<Picture> pictures = new List<Picture>();
            pictures.AddRange(profilesWithImages.Select(x=>x.Picture));
            pictures.AddRange(customersWithImages.Select(x => x.Picture));

            List<PictureInfoDTO> notUsed = new List<PictureInfoDTO>();
            var dictionaryImages=pictures.ToDictionary(x => x.PictureId.ToString());
            ServiceConfiguration configuration = new ServiceConfiguration();
            var files=Directory.GetFiles(configuration.ImagesFolder);

            foreach (var file in files)
            {
                var filename=Path.GetFileName(file);
                if (string.IsNullOrEmpty(Path.GetExtension(filename)))
                {
                    bool isUsed = dictionaryImages.ContainsKey(filename);
                    if (!isUsed)
                    {
                        notUsed.Add(new PictureInfoDTO(new Guid(filename), ""));
                    }
                }
            }
            if (!param.OnlyShowUsers)
            {
                foreach (var pictureInfoDto in notUsed)
                {
                    PictureService pictureService = new PictureService(NHibernateContext.Current().Session, null, configuration);
                    pictureService.DeletePicture(pictureInfoDto.Map<Picture>());
                }
            }
            return notUsed;
        }

        public IList<UserDTO> DeleteOldProfiles(DeleteOldProfilesParam param)
        {
            //ProfileStatistics stat = null;
            //BAPoints point = null;
            //var unusedProfiles =
            //    NHibernateContext.Current().Session.QueryOver<Profile>().JoinAlias(x => x.Statistics, () => stat).JoinAlias(x => x.BAPoints, () => point)
            //        .Where(x => stat.TrainingDaysCount == 0 && stat.WorkoutPlansCount == 0 &&
            //            stat.SupplementsDefinitionsCount == 0 && stat.LastLoginDate < DateTime.Now.AddMonths(-8)
            //            && !x.IsDeleted && x.UserName != "Admin" && x.BAPoints.Count==0).List();
            var unusedProfiles = NHibernateContext.Current()
                .Session.Query<Profile>()
                .Where(x => x.Statistics.TrainingDaysCount == 0 && x.Statistics.WorkoutPlansCount == 0 &&
                            x.Statistics.SupplementsDefinitionsCount == 0 &&
                            x.Statistics.LastLoginDate < DateTime.Now.AddMonths(-8)
                            && !x.IsDeleted && x.UserName != "Admin" && !x.BAPoints.Any()).ToList();
            if (!param.OnlyShowUsers)
            {
                ServiceConfiguration configuration = new ServiceConfiguration();
                ProfileService service = new ProfileService(NHibernateContext.Current().Session, null, configuration, null, null, null);
                foreach (var unusedProfile in unusedProfiles)
                {
                    try
                    {
                        using (var trans = NHibernateContext.Current().Session.BeginSaveTransaction())
                        {
                            service.deleteProfile(NHibernateContext.Current().Session, unusedProfile);
                            if (unusedProfile.Picture != null)
                            {

                                PictureService pictureService = new PictureService(NHibernateContext.Current().Session, null, configuration);
                                pictureService.DeletePicture(unusedProfile.Picture);
                            }
                            trans.Commit();


                        }
                    }
                    catch (Exception ex)
                    {
                        BodyArchitect.Logger.ExceptionHandler.Default.Process(ex);
                        throw;
                    }
                    
                    
                }
                
                
            }
            return unusedProfiles.Map<IList<UserDTO>>();
        }

        public void DeleteOrphandExerciseRecords(DeleteOldProfilesParam param)
        {
            Serie serie = null;
            ExerciseProfileData profileData = null;
            var unusedProfiles = NHibernateContext.Current().Session.QueryOver<ExerciseProfileData>().JoinAlias(x => x.Serie, () => serie)
                .JoinAlias(x => serie.ExerciseProfileData, () => profileData)
                .Where(x => profileData.GlobalId != x.GlobalId).List();

            if (!param.OnlyShowUsers)
            {
                using (var trans = NHibernateContext.Current().Session.BeginSaveTransaction())
                        {
                    foreach (var unusedProfile in unusedProfiles)
                    {
                        try
                        {
                        
                                if (unusedProfile.GlobalId != unusedProfile.Serie.ExerciseProfileData.GlobalId)
                                {
                                    NHibernateContext.Current().Session.Delete(unusedProfile);
                                }
                            
                        }
                        catch (Exception ex)
                        {
                            BodyArchitect.Logger.ExceptionHandler.Default.Process(ex);
                            throw;
                        }


                    }
                trans.Commit();


                        }


            }
        }
    }
}
