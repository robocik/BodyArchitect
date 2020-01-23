using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BodyArchitect.DataAccess.NHibernate;
using BodyArchitect.Logger;
using BodyArchitect.Model;
using BodyArchitect.Portable;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using ICSharpCode.SharpZipLib.Zip;
using NHibernate;
using Newtonsoft.Json;
using EntryObjectStatus = BodyArchitect.Model.EntryObjectStatus;
using ObjectNotFoundException = BodyArchitect.Shared.ObjectNotFoundException;

namespace BodyArchitect.Service.V2.Services
{
    public class GPSTrackerService : ServiceBase
    {
        public GPSTrackerService(ISession session, SecurityInfo securityInfo, ServiceConfiguration configuration) : base(session, securityInfo, configuration)
        {
        }

        //http://stackoverflow.com/questions/472906/net-string-to-byte-array-c-sharp
        //public PagedResult<GPSPoint> GetGPSCoordinates(Guid entryId,PartialRetrievingInfo pageInfo)
        //{
        //    Log.WriteWarning("GetGPSCoordinates:Username={0},gpsTrackerEntryId={1}", SecurityInfo.SessionData.Profile.UserName, entryId);

        //    using (var transactionScope = Session.BeginGetTransaction())
        //    {
        //        var res = Session.QueryOver<GPSTrackerEntry>().Fetch(x => x.Coordinates).Eager.Where(x => x.GlobalId == entryId).SingleOrDefault();
        //        if (res.Coordinates == null)
        //        {
        //            return new PagedResult<GPSPoint>(new List<GPSPoint>(),0,0 );
        //        }
        //        char[] chars = new char[res.Coordinates.Content.Length / sizeof(char)];
        //        System.Buffer.BlockCopy(res.Coordinates.Content, 0, chars, 0, res.Coordinates.Content.Length);

        //        var pointsCollection = JsonConvert.DeserializeObject<IList<GPSPoint>>(new string(chars));

        //        transactionScope.Commit();
        //        return pointsCollection.ToPagedResult(pageInfo);
        //    }
        //}
        
        //public GPSTrackerEntryDTO GPSCoordinatesOperation(GPSCoordinatesOperationParam param)
        //{
        //    Log.WriteWarning("GPSCoordinatesOperation:Username={0},gpsTrackerEntryId={1}", SecurityInfo.SessionData.Profile.UserName, param.GPSTrackerEntryId);

        //    using (var transactionScope = Session.BeginGetTransaction())
        //    {
        //        var res = Session.QueryOver<GPSTrackerEntry>().Fetch(x => x.Coordinates).Eager.SingleOrDefault();

        //        if(param.Operation==GPSCoordinatesOperationType.DeleteCoordinates)
        //        {
        //            var coordinates = res.Coordinates;
        //            res.Coordinates = null;
        //            if(coordinates!=null)
        //            {
        //                Session.Delete(coordinates);
        //            }
        //        }
        //        else
        //        {
        //            string output = JsonConvert.SerializeObject(param.Coordinates);
        //            var bytes = UTF8Encoding.UTF8.GetBytes(output);
        //            if (res.Coordinates == null)
        //            {
        //                res.Coordinates = new GPSCoordinates();
        //            }
        //            res.Coordinates.Content = bytes.ToZip();
        //        }
                
        //        Session.SaveOrUpdate(res);
        //        transactionScope.Commit();
        //        return res.Map<GPSTrackerEntryDTO>();
        //    }
            

        //}

        public GPSCoordinatesOperationResult GPSCoordinatesOperation(GPSOperationParam param)
        {
            Log.WriteWarning("GPSCoordinatesOperation:Username={0},gpsTrackerEntryId={1}", SecurityInfo.SessionData.Profile.UserName, param.Params.GPSTrackerEntryId);

            using (var transactionScope = Session.BeginGetTransaction())
            {
                var dbProfile = Session.Load<Profile>(SecurityInfo.SessionData.Profile.GlobalId);

                var res = Session.QueryOver<GPSTrackerEntry>().Fetch(x => x.Coordinates).Eager.Where(x=>x.GlobalId==param.Params.GPSTrackerEntryId).SingleOrDefault();
                if (res == null)
                {
                    throw new ObjectNotFoundException();
                }
                if (res.TrainingDay.Profile != dbProfile)
                {
                    throw new CrossProfileOperationException();
                }
                if (param.Params.Operation == GPSCoordinatesOperationType.DeleteCoordinates)
                {
                    deletesCoordinates(res);
                }
                else
                {
                    try
                    {
                        MemoryStream zippedStream = new MemoryStream();
                        param.CoordinatesStream.CopyTo(zippedStream);
                        zippedStream.Seek(0, SeekOrigin.Begin);
                        if (zippedStream.Length == 0)
                        {
                            deletesCoordinates(res);
                        }
                        else
                        {
                            int maxPointNumber = 21600;//points after 24h, one point every 4 sec
                            var unzippedData = zippedStream.FromZip();
                            var json = UTF8Encoding.UTF8.GetString(unzippedData);
                            var points = JsonConvert.DeserializeObject<IList<GPSPoint>>(json).OrderBy(x=>x.Duration).ToList();
                            if (points.Count == 0)
                            {
                                deletesCoordinates(res);
                            }
                            else if(points.Count>maxPointNumber)
                            {
                                throw new ArgumentOutOfRangeException("You can upload max "+maxPointNumber+" points");
                            }
                            else
                            {
                                if (param.Params.Operation ==GPSCoordinatesOperationType.UpdateCoordinatesWithCorrection)
                                {//correct altitude data
                                    GPSTrackerHelper.CorrectGpsData(points);
                                }
                                performGpsPointsCalculations(res, points, dbProfile);

                                if (SecurityInfo.Licence.IsPremium)
                                {//for premium account store coordinates in the db
                                    if (res.Coordinates == null)
                                    {
                                        res.Coordinates = new GPSCoordinates();
                                    }
                                    if (param.Params.Operation ==GPSCoordinatesOperationType.UpdateCoordinatesWithCorrection)
                                    {
//now zip corrected data again and save them to the db
                                        var settings = new JsonSerializerSettings();
                                        settings.NullValueHandling = NullValueHandling.Ignore;
                                        settings.MissingMemberHandling = MissingMemberHandling.Ignore;
                                        json = JsonConvert.SerializeObject(points, settings);
                                        var bytes = UTF8Encoding.UTF8.GetBytes(json);
                                        res.Coordinates.Content = bytes.ToZip();
                                    }
                                    else
                                    {
                                        res.Coordinates.Content = zippedStream.ToArray();
                                    }
                                }
                            }
                        }
                    }
                    catch(ZipException ex)
                    {
                        throw new ConsistencyException(ex.Message, ex);
                    }
                    catch (JsonSerializationException ex)
                    {
                        throw  new ConsistencyException(ex.Message,ex);
                    }
                    

                }

                Session.SaveOrUpdate(res);
                transactionScope.Commit();
                return new GPSCoordinatesOperationResult() { GPSTrackerEntry = res.Map<GPSTrackerEntryDTO>() };

            }
        }


        private static void performGpsPointsCalculations(GPSTrackerEntry res, List<GPSPoint> points, Profile dbProfile)
        {
            
            if (res.Status == EntryObjectStatus.Planned)
            {
                res.Status = EntryObjectStatus.Done;
            }
            //perform calculation based on gps coordinates
            res.Distance = (decimal?) GPSTrackerHelper.GetDistance(points);
            var pointsWithSpeed = points.Where(x => !float.IsNaN(x.Speed) && x.Speed > -1).ToList();
            if (pointsWithSpeed.Count > 0)
            {
                res.MaxSpeed = (decimal?) pointsWithSpeed.Max(x => x.Speed);
            }
            //res.AvgSpeed = (decimal?) points.Average(x => x.Speed);
            var pointsWithAltitude = points.Where(x => !float.IsNaN(x.Altitude) && x.Altitude > -1).ToList();
            if (pointsWithAltitude.Count > 0)
            {
                res.MaxAltitude = (decimal?) pointsWithAltitude.Max(x => x.Altitude);
                res.MinAltitude = (decimal?) pointsWithAltitude.Min(x => x.Altitude);
                res.TotalAscent = (decimal?) GPSTrackerHelper.GetTotalAscents(pointsWithAltitude);
                res.TotalDescent = (decimal?) GPSTrackerHelper.GetTotalDescends(pointsWithAltitude);
            }

            if (res.Duration == null || res.Duration == 0)
            {
                res.Duration = (decimal?) points.Last().Duration;
            }
            if (res.Calories == null || res.Calories == 0)
            {
                TrainingDayService.CalculateCaloriesBurned(res,
                                                           res.TrainingDay.Customer != null
                                                               ? (IPerson) res.TrainingDay.Customer
                                                               : dbProfile);
            }
            if (res.Duration.Value > 0)
            {
                res.AvgSpeed = res.Distance.Value/res.Duration.Value;
            }
        }

        private void deletesCoordinates(GPSTrackerEntry res)
        {
            var coordinates = res.Coordinates;
            res.Coordinates = null;
            if (coordinates != null)
            {
                Session.Delete(coordinates);
            }
        }

        public GPSCoordinatesDTO GetGPSCoordinates(GetGPSCoordinatesParam param)
        {
            Log.WriteWarning("GetGPSCoordinates:Username={0},entryId={1}", SecurityInfo.SessionData.Profile.UserName, param.GPSTrackerEntryId);

            var dto = new GPSCoordinatesDTO();
            var dbProfile = Session.Load<Profile>(SecurityInfo.SessionData.Profile.GlobalId);
            var res = Session.QueryOver<GPSTrackerEntry>().Fetch(x => x.TrainingDay).Eager.Fetch(x => x.Coordinates).Eager.Where(x => x.GlobalId == param.GPSTrackerEntryId).SingleOrDefault();
            if (res.TrainingDay.Profile != dbProfile)
            {
                throw new CrossProfileOperationException();
            }
            var gps = res.Coordinates;
            if (gps == null)
            {
                throw new ObjectNotFoundException("This entry doesn't have gps coordinates");
            }
            MemoryStream memoryStream = new MemoryStream(gps.Content);
            memoryStream.Seek(0, SeekOrigin.Begin);
            dto.Stream = memoryStream;
            return dto;
        }
    }
}
