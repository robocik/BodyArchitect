using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.UI.Cache;
using BodyArchitect.Service.V2.Model.Instructor;

namespace BodyArchitect.Client.Module.Instructor.ViewModel
{
    public class ScheduleEntryReservationViewModel:ViewModelBase
    {
        private ScheduleEntryReservationDTO reservation;
        

        public ScheduleEntryReservationViewModel(ScheduleEntryReservationDTO reservation)
        {
            this.reservation = reservation;
        }

        public string Customer
        {
            get 
            {
                var customer = CustomersReposidory.Instance.GetItem(Reservation.CustomerId);
                if(customer!=null)
                {
                    return customer.FullName;
                }
                return "ScheduleEntryReservationViewModel_Customer_Deleted".TranslateInstructor();
            }
        }



        public string PaidIcon
        {
            get { return reservation.IsPaid ? "/BodyArchitect.Client.Module.Instructor;component/Images/Paid.png" : null; }
        }

        public ScheduleEntryReservationDTO Reservation
        {
            get { return reservation; }
        }

        
    }
}
