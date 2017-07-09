using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EasyPark.WebUI.Areas.Manager.ViewModels.Ticket
{
    public class ReturnListViewModel
    {
        public long ID { get; set; }
        [Display(Name = "Ticket number")]
        public string TicketNumber { get; set; }
        [Display(Name = "License plate")]
        public string LicensePlate { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }
        [Display(Name = "Customer Phone")]
        public string CustomerPhone { get; set; }
    }
}