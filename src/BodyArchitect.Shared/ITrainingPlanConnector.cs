using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Shared
{
    public interface ITrainingPlanConnector
    {
        Guid? TrainingPlanItemId { get; set; }
    }
}
