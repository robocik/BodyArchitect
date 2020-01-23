using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls.Animations;

namespace BodyArchitect.WP7.Pages
{
    public abstract class SimpleEntryObjectPage<T> : AnimatedBasePage where T:EntryObjectDTO
    {
        public T Entry
        {
            get { return ApplicationState.Current.TrainingDay.TrainingDay.GetEntry<T>(ApplicationState.Current.CurrentEntryId); }
        }

        public bool EditMode
        {
            get { return ApplicationState.Current.TrainingDay.TrainingDay.IsMine && Entry.Status != EntryObjectStatus.System; }
        }
    }

    public abstract class StrengthTrainingPageBase:SimpleEntryObjectPage<StrengthTrainingEntryDTO>
    {
        
    }

    public abstract class SupplementsPageBase : SimpleEntryObjectPage<SuplementsEntryDTO>
    {

    }

    public abstract class GPSPageBase : SimpleEntryObjectPage<GPSTrackerEntryDTO>
    {

    }

    public abstract class MeasurementsPageBase : SimpleEntryObjectPage<SizeEntryDTO>
    {

    }

    public abstract class BlogPageBase : SimpleEntryObjectPage<BlogEntryDTO>
    {

    }
}
