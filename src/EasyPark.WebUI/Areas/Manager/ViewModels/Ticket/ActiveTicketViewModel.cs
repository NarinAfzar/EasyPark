using EasyPark.WebUI.DAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EasyPark.WebUI.Areas.Manager.ViewModels.Ticket
{
    public class ActiveTicketViewModel
    {
        public long ID { get; set; }
        [Display(Name = "Ticket Number")]
        public string TicketNumber { get; set; }
        [Display(Name = "Plate Number")]
        public string Plate { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
        public TicketStatusType Status { get; set; }
        [DataType(DataType.Time)]
        [Display(Name = "Last Activity")]
        public DateTime LastActivityDate { get; set; }
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }
        [Display(Name = "Customer Phone")]
        public string CustomerPhone { get; set; }

    }
}