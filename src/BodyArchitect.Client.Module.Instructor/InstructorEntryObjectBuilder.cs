using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Client.Common.Plugins;
using BodyArchitect.Client.UI.Views.MyPlace;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;

namespace BodyArchitect.Client.Module.Instructor
{
    [Serializable]
    class InstructorEntryObjectBuilder : IEntryObjectBuilderProvider
    {
        private ScheduleEntryReservationDTO reservation;
        private ScheduleEntryDTO scheduleEntry;

        public InstructorEntryObjectBuilder(ScheduleEntryReservationDTO reservation, ScheduleEntryDTO scheduleEntry)
        {
            this.reservation = reservation;
            this.scheduleEntry = scheduleEntry;
        }

        public void EntryObjectCreated(EntryObjectDTO entryObject)
        {
            if(reservation!=null)
            {
                entryObject.ReservationId = reservation.GlobalId;
            }
            StrengthTrainingEntryDTO strength = entryObject as StrengthTrainingEntryDTO;
            SizeEntryDTO size = entryObject as SizeEntryDTO;
            if(strength!=null)
            {
                strength.StartTime = scheduleEntry.StartTime.ToLocalTime();
                strength.EndTime = scheduleEntry.EndTime.ToLocalTime();
                if (scheduleEntry.MyPlaceId.HasValue)
                {
                    var myPlacesCache = MyPlacesReposidory.GetCache(null);
                    strength.MyPlace = myPlacesCache.GetItem(scheduleEntry.MyPlaceId.Value);
                }
            }
            if (size != null)
            {
                size.Wymiary.Time.DateTime = scheduleEntry.StartTime.ToLocalTime();
            }
        }
    }
}
