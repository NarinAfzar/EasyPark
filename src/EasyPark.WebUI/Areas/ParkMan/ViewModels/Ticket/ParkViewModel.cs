using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace EasyPark.WebUI.Areas.ParkMan.ViewModels.Ticket
{
    public class ParkViewModel
    {
        public long ID { get; set; }

        [Display(Name = "Ticket number:")]
        public string TicketNumber { get; set; }

        [Display(Name = "Description:")]
        public string Description { get; set; }

        [Display(Name = "License plate:")]
        public string LicensePlate { get; set; }

        public IEnumerable<SelectListItem> AvailableSolts { set; get; }

        [Display(Name = "Parking slot:")]
        public long Slot { set; get; }
    }
}