using System;

namespace EasyPark.WebUI.Areas.Manager.ViewModels.Ticket
{
    public class TicketViewModel
    {
        public TimeSpan SetDepartureTime
        {
            set
            {
                DepartureTime = value == default(TimeSpan) ? "00:00" : value.ToString(@"hh\:mm");
            }
        }

        public string LicensePlate { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Color { get; set; }
        public string DepartureTime { get; private set; }

    }
}