using System.ComponentModel.DataAnnotations;

namespace EasyPark.WebUI.Areas.ParkMan.ViewModels.Ticket
{
    public class RequestListViewModel
    {
        public long ID { get; set; }

        [Display(Name = "Ticket number")]
        public string TicketNumber { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "License plate")]
        public string LicensePlate { get; set; }

        [Display(Name = "Parking slot")]
        public string SlotNumber { get; set; }
    }
}