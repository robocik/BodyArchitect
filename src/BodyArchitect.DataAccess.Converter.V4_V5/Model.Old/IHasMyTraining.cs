using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Model.Old
{
    public interface IHasMyTraining
    {
        Guid? MyTrainingId
        {
            get;
            set;
        }
        
    }
}
