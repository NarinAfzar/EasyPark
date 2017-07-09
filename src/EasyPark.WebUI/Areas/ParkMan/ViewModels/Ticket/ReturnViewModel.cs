using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace EasyPark.WebUI.Areas.ParkMan.ViewModels.Ticket
{
    public class ReturnViewModel
    {
        public long ID { get; set; }

        [Display(Name = "Ticket number:")]
        public string TicketNumber { get; set; }

        [Display(Name = "Description:")]
        public string Description { get; set; }

        [Display(Name = "License plate:")]
        public string LicensePlate { get; set; }

        public IEnumerable<SelectListItem> AvailableDrivers { set; get; }

        [Display(Name = "Driver:")]
        public string Driver { set; get; }

        [Display(Name = "Parking slot:")]
        public string SlotNumber { set; get; }
    }
}