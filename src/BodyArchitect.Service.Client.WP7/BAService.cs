using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using BodyArchitect.Portable;
using BodyArchitect.Portable.Exceptions;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using BodyArchitect.WP7.Client.WCF;
using Microsoft.Phone.Net.NetworkInformation;
using Newtonsoft.Json;
using NetworkInterface = System.Net.NetworkInformation.NetworkInterface;

namespace BodyArchitect.Service.Client.WP7
{
    public static class BAService
    {
        static void exceptionHandling(Action method)
        {
            exceptionHandling<ProfileDTO>(delegate
            {
                method();
                return null;
            });
        }

        async static Task<T> exceptionHandling<T>(Func<IBodyArchitectAccessService, Task<T>> method, bool withoutLogging=false)
        {
            if (!NetworkInterface.GetIsNetworkAvailable() || !withoutLogging && ApplicationState.Current.IsOffline)
            {
                throw new NetworkException(NetworkError.SocketNotConnected);
            }
            try
            {
                bool retry = true;
                Exception exception = null;
                IBodyArchitectAccessService client = ApplicationState.CreateService();



                do
                {
                    OperationContextScope scope =
                        new OperationContextScope(((BodyArchitectAccessServiceClient)client).InnerChannel);
                    ApplicationState.AddCustomHeaders();
                    try
                    {
                        return await method(client);

                    }
                    catch (FaultException<BAAuthenticationException> ex)
                    {
                        if (!retry)
                        {
                            throw ex;
                        }
                        retry = false;
                        exception = ex;
                    }

                    scope = new OperationContextScope(((BodyArchitectAccessServiceClient)client).InnerChannel);
                    ApplicationState.AddCustomHeaders();
                    ClientInformation info = Settings.GetClientInformation();

                    var sessionData =
                        await
                        Task<SessionData>.Factory.FromAsync(client.BeginLogin, client.EndLogin, info,
                                                            ApplicationState.Current.TempUserName,
                                                            ApplicationState.Current.TempPassword, null);

                    if (sessionData == null)
                    {
                        throw exception;
                    }
                    ApplicationState.Current.SessionData = sessionData;
                    ApplicationState.Current.SessionData.Token.Language = ApplicationState.CurrentServiceLanguage;
                } while (true);
                return default(T);
            }
            catch (FaultException<ValidationFault> ex)
            {
                throw new ValidationException(ex.Detail.ToValidationResults());
            }
            catch (FaultException<BAAuthenticationException> baEx)
            {
                throw new Portable.Exceptions.AuthenticationException(baEx.Detail.Message);
            }
            catch (FaultException<BAServiceException> baEx)
            {
                switch (baEx.Detail.ErrorCode)
                {
                    case ErrorCode.ProfileRankException:
                        throw new ProfileRankException(baEx.Detail.Message);
                    case ErrorCode.ProfileIsNotActivatedException:
                        throw new ProfileIsNotActivatedException(baEx.Detail.Message);
                    case ErrorCode.UserDeletedException:
                        throw new UserDeletedException(baEx.Detail.Message);
                    case ErrorCode.MaintenanceException:
                        throw new MaintenanceException(baEx.Detail.Message);
                    case ErrorCode.ProfileDeletedException:
                        throw new ProfileDeletedException(baEx.Detail.Message);
                    case ErrorCode.ConsistencyException:
                        throw new ConsistencyException(baEx.Detail.Message);
                    case ErrorCode.SecurityException:
                        throw new SecurityException(baEx.Detail.Message);
                    case ErrorCode.EMailSendException:
                        throw new EMailSendException(baEx.Detail.Message);
                    case ErrorCode.DeleteConstraintException:
                        throw new DeleteConstraintException(baEx.Detail.Message);
                    case ErrorCode.UniqueException:
                        throw new UniqueException(baEx.Detail.Message);
                    case ErrorCode.CrossProfileOperation:
                        throw new CrossProfileOperationException(baEx.Detail.Message);
                    case ErrorCode.ValidationException:
                        throw new ValidationException(baEx.Detail.Message);
                    case ErrorCode.AuthenticationException:
                        throw new Portable.Exceptions.AuthenticationException(baEx.Detail.Message);
                    case ErrorCode.InvalidOperationException:
                        throw new InvalidOperationException(baEx.Detail.Message);
                    case ErrorCode.ArgumentNullException:
                        throw new ArgumentNullException(baEx.Detail.AdditionalData, baEx.Detail.Message);
                    case ErrorCode.ArgumentOutOfRange:
                        throw new ArgumentOutOfRangeException(baEx.Detail.AdditionalData, baEx.Detail.Message);
                    case ErrorCode.ProductAlreadyPaid:
                        throw new ProductAlreadyPaidException(baEx.Detail.Message);
                    case ErrorCode.ArgumentException:
                        throw new ArgumentException(baEx.Detail.Message);
                    case ErrorCode.DatabaseException:
                        throw new DatabaseException(baEx.Detail.Message);
                    case ErrorCode.NullReferenceException:
                        throw new NullReferenceException(baEx.Detail.Message);
                    case ErrorCode.DatabaseVersionException:
                        throw new DatabaseVersionException(baEx.Detail.Message);
                    case ErrorCode.OldDataException:
                        throw new OldDataException(baEx.Detail.Message);
                    case ErrorCode.CannotAcceptRejectInvitationDoesntExistException:
                        throw new CannotAcceptRejectInvitationDoesntExistException(baEx.Detail.Message);
                    case ErrorCode.ProfileAlreadyFriendException:
                        throw new ProfileAlreadyFriendException(baEx.Detail.Message);
                    case ErrorCode.ObjectIsFavoriteException:
                        throw new ObjectIsFavoriteException(baEx.Detail.Message);
                    case ErrorCode.ObjectIsNotFavoriteException:
                        throw new ObjectIsNotFavoriteException(baEx.Detail.Message);
                    case ErrorCode.ObjectNotFound:
                        throw new ObjectNotFoundException(baEx.Detail.Message);
                    case ErrorCode.UnauthorizedAccessException:
                        throw new UnauthorizedAccessException(baEx.Detail.Message);
                    case ErrorCode.TrainingIntegrityException:
                        throw new TrainingIntegrationException(baEx.Detail.Message);
                    case ErrorCode.FileNotFoundException:
                        throw new FileNotFoundException(baEx.Detail.Message);
                    case ErrorCode.AlreadyOccupied:
                        throw new AlreadyOccupiedException(baEx.Detail.Message);
                    case ErrorCode.LicenceException:
                        throw new LicenceException(baEx.Detail.Message);

                }
                throw new Exception(baEx.Detail.Message);
            }

        }

        static async public Task<GPSCoordinatesOperationResult> GPSCoordinatesOperationAsync(Guid gpsTrackerEntryId, GPSCoordinatesOperationType operationType, IEnumerable<GPSPoint> points)
        {
            GPSOperationParam dto = new GPSOperationParam();
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.MissingMemberHandling = MissingMemberHandling.Ignore;
            var json = JsonConvert.SerializeObject(points, settings);
            var bytes = UTF8Encoding.UTF8.GetBytes(json);
            dto.CoordinatesStream = bytes.ToZip();

            GPSOperationData param = new GPSOperationData();
            param.GPSTrackerEntryId = gpsTrackerEntryId;
            param.Operation = GPSCoordinatesOperationType.UpdateCoordinates;

            var test = exceptionHandling((client) =>
            {
                OperationContext.Current.OutgoingMessageHeaders.Add(MessageHeader.CreateHeader("SessionId", "http://MYBASERVICE.TK/", ApplicationState.Current.SessionData.Token.SessionId));
                OperationContext.Current.OutgoingMessageHeaders.Add(MessageHeader.CreateHeader("Params", "http://MYBASERVICE.TK/", param));
                return Task<GPSCoordinatesOperationResult>.Factory.FromAsync(client.BeginGPSCoordinatesOperation, client.EndGPSCoordinatesOperation, dto, null);
            });
            return await test;
        }

        static async public Task<TrainingDayDTO> GetTrainingDayAsync(WorkoutDayGetOperation param, RetrievingInfo retrievingInfo)
        {
            var test = exceptionHandling((client) =>
            {
                return Task<TrainingDayDTO>.Factory.FromAsync(client.BeginGetTrainingDay, client.EndGetTrainingDay,ApplicationState.Current.SessionData.Token, param, retrievingInfo, null);
            });
            return await test;
        }

        static async public Task<SaveTrainingDayResult> SaveTrainingDayAsync(TrainingDayDTO day)
        {
            var test = exceptionHandling((client) =>
            {
                return Task<SaveTrainingDayResult>.Factory.FromAsync(client.BeginSaveTrainingDay, client.EndSaveTrainingDay, ApplicationState.Current.SessionData.Token, day, null);
            });
            return await test;
        }

        static async public Task<SessionData> LoginAsync( string username, string password)
        {
            ClientInformation info = Settings.GetClientInformation();
            var test = exceptionHandling((client) =>
            {
                return Task<SessionData>.Factory.FromAsync(client.BeginLogin, client.EndLogin, info, username, password, null);
            },true);
            return await test;
        }

        static async public Task<ProfileInformationDTO> GetProfileInformationAsync(GetProfileInformationCriteria criteria)
        {
            var test = exceptionHandling((client) =>
            {
                return Task<ProfileInformationDTO>.Factory.FromAsync(client.BeginGetProfileInformation, client.EndGetProfileInformation, ApplicationState.Current.SessionData.Token, criteria, null);
            });
            return await test;
        }

        static async public Task<List<GPSPoint>> GetGPSCoordinatesAsync(Guid gpsTrackerEntryId)
        {
            var test = exceptionHandling((client) =>
            {
                GetGPSCoordinatesParam param = new GetGPSCoordinatesParam();
                OperationContext.Current.OutgoingMessageHeaders.Add(MessageHeader.CreateHeader("SessionId", "http://MYBASERVICE.TK/", ApplicationState.Current.SessionData.Token.SessionId));
                OperationContext.Current.OutgoingMessageHeaders.Add(MessageHeader.CreateHeader("GPSTrackerEntryId", "http://MYBASERVICE.TK/", gpsTrackerEntryId));
                return Task<GPSCoordinatesDTO>.Factory.FromAsync(client.BeginGetGPSCoordinates, client.EndGetGPSCoordinates,param, null);
            });
            var result= await test;
            
            var unzipped = result.Stream.FromZip();
            var json = UTF8Encoding.UTF8.GetString(unzipped, 0, unzipped.Length);
            var points = JsonConvert.DeserializeObject<List<GPSPoint>>(json);
            return points;
        }

        static async public Task<PagedResultOfTrainingDayDTO5oAtqRlh> GetTrainingDaysAsync(WorkoutDaysSearchCriteria search, PartialRetrievingInfo pageInfo)
        {
            var test = exceptionHandling((client) =>
            {
                return Task<PagedResultOfTrainingDayDTO5oAtqRlh>.Factory.FromAsync(client.BeginGetTrainingDays, client.EndGetTrainingDays, ApplicationState.Current.SessionData.Token, search, pageInfo, null);
            });
            return await test;
        }

        static async public Task<bool> CheckProfileNameAvailabilityAsync(string username)
        {
            var test = exceptionHandling((client) =>
            {
                return Task<bool>.Factory.FromAsync(client.BeginCheckProfileNameAvailability, client.EndCheckProfileNameAvailability, username, null);
            },true);
            return await test;
        }
    }
}
