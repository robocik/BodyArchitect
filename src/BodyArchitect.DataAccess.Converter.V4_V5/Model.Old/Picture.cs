using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Model.Old
{
    public class Picture
    {
        public Picture()
        {
            
        }
        public Picture(Guid pictureId, string hash)
        {
            PictureId = pictureId;
            Hash = hash;
        }

        public virtual Guid PictureId { get; set; }

        public virtual string Hash { get; set; }

    }
}
