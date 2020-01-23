using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace BodyArchitect.Service.V2.Model
{
    [MessageContract]
    [Serializable]
    public class PictureDTO : PictureInfoDTO
    {
        public PictureDTO() : base(Guid.Empty,null)
        {
        }

        public PictureDTO(IToken token)
            : base(token)
        {
        }

        public PictureDTO(Guid pictureId, string hash) : base(pictureId, hash)
        {
        }

        [MessageBodyMember(Order = 1)]
        public Stream ImageStream { get; set; }

        public PictureInfoDTO GetPictureInfo()
        {
            return new PictureInfoDTO(PictureId,Hash);
        }

    }

}
