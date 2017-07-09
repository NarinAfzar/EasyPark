using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EasyPark.WebUI.Areas.Manager.ViewModels.Ticket
{
    public class AssignDriverViewModel
    {
        public string DriverId { set; get; }
        public long TicketId { set; get; }
        [Display(Name ="Driver:")]
        public IEnumerable<SelectListItem> Drivers { get; set; }
    }
}