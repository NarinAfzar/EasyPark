using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace EasyPark.WebUI.Areas.Manager.ViewModels.Ticket
{
    public class CreateViewModel
    {
        [Display(Name = "Phone Number")]
        [Required(ErrorMessage ="Required! Please enter phone number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Driver")]        
        public string Driver { get; set; }

        [Display(Name = "Car Color")]
        public string Color { get; set; }

        [Display(Name = "Plate Number")]
        [Required(ErrorMessage = "Required! Please enter car plate")]
        public string Plate { get; set; }
        [Display(Name = "Ticket Number")]
        [Required(ErrorMessage = "Required! Please enter ticket number")]
        public string TicketNumber { get; set; }
        [Display(Name = "Departure Time")]
        [Required(ErrorMessage = "Required! Please enter departure time")]
        public TimeSpan DepartureTime { get; set; }
        public bool Status { get; set; }

        public IEnumerable<SelectListItem> Drivers { get; set; }
    }
}