using System;
using System.ComponentModel.DataAnnotations;

namespace EasyPark.WebUI.Areas.ParkMan.ViewModels.Ticket
{
    public class ListItemViewModel
    {
        public long ID { get; set; }

        [Display(Name = "Ticket number")]
        public string TicketNumber { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Licence plate")]
        public string LicensePlate { get; set; }

        [Display(Name = "Departure time")]
        [DataType(DataType.Time)]
        public TimeSpan? DepartureTime { get; set; }
    }
}