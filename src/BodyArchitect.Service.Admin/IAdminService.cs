using System;
using System.Collections.Generic;
using System.ServiceModel;
using BodyArchitect.Service.Admin.Objects;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Service.Admin
{
    [ServiceContract(Namespace = Const.ServiceNamespaceRoot)]
    public interface IAdminService
    {
        [OperationContract]
        IList<UserDTO> DeleteOldProfiles(DeleteOldProfilesParam param);

        [OperationContract]
        void SendMessage(string topic, string message, SendMessageMode mode, List<int> countriesId);

        [OperationContract]
        IList<PictureInfoDTO> DeleteUnusedImages(DeleteOldProfilesParam param);

        [OperationContract]
        void DeleteOrphandExerciseRecords(DeleteOldProfilesParam param);
    }
}
