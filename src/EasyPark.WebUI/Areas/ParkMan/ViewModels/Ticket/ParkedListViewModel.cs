using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EasyPark.WebUI.Areas.ParkMan.ViewModels.Ticket
{
    public class ParkedListViewModel
    {
        public long ID { get; set; }
        [Display(Name = "Ticket Number")]
        public string TicketNumber { get; set; }
        [Display(Name = "Plate Number")]
        public string Plate { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Display(Name = "Requested at")]
        [DataType(DataType.Time)]
        public DateTime? RequestTime { get; set; }
        [Display(Name = "Departue time")]
        [DataType(DataType.Time)]
        public TimeSpan? DepartureTime { get; set; }
        [Display(Name = "Park date")]
        [DataType(DataType.Time)]
        public DateTime ParkDate { get; set; }
        [Display(Name = "Parking slot")]
        public string ParkingSlot { get; set; }
    }
}