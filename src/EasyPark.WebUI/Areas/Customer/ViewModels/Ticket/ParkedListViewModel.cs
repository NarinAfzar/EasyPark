using EasyPark.WebUI.DAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EasyPark.WebUI.Areas.Customer.ViewModels.Ticket
{
    public class ParkedListViewModel
    {
        public long ID { set; get; }
        [Display(Name = "Ticket number")]
        public string TicketNumber { set; get; }
        [Display(Name = "License plate")]
        public string LicensePlate { set; get; }
        [Display(Name = "Departue time")]
        [DataType(DataType.Time)]
        public TimeSpan? DepartureDate { set; get; }
        public bool IsRequested { set; get; }
    }
}