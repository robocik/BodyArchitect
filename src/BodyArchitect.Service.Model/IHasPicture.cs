using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace BodyArchitect.Service.Model
{
    public interface IHasPicture
    {
        PictureInfoDTO Picture { get; set; }
    }

    [MessageContract]
    [Serializable]
    public class PictureInfoDTO:ICloneable,IToken
    {
        public PictureInfoDTO()
        {
        }

        public PictureInfoDTO(IToken token)
        {
            SessionId = token.SessionId;
        }

        public PictureInfoDTO(Guid pictureId, string hash)
        {
            PictureId = pictureId;
            Hash = hash;
        }

        [MessageHeader(MustUnderstand = true)]
        public Guid PictureId { get; set; }

        [MessageHeader(MustUnderstand = true)]
        public string Hash { get; set; }

        public object Clone()
        {
            PictureInfoDTO info = new PictureInfoDTO(PictureId,Hash);
            return info;
        }

        [MessageHeader(MustUnderstand = true)]
        public Guid SessionId
        {
            get; set;
        }
    }
}
