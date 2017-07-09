using System;
using System.ComponentModel.DataAnnotations;

namespace EasyPark.WebUI.Areas.CarDriver.ViewModels
{
    public class TaskViewModel
    {
        [Display(Name = "Assign Date")]
        public DateTime CreationDate { get; set; }

        [Display(Name = "Ticket ID")]
        public long TicketID { get; set; }

        [Display(Name = "Ticket Status")]
        public string Type { get; set; }

        public long StatusId { get; set; }
    }
}