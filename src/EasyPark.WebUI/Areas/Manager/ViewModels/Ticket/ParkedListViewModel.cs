using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EasyPark.WebUI.Areas.Manager.ViewModels.Ticket
{
    public class ParkedListViewModel
    {
        public DateTime? SetRequest
        {
            set
            {
                if(value==null)
                {
                    Requested = false;
                    return;
                }
                Requested = true;
                RequestTime = value.Value.ToString("HH:mm");
            }
        }
        public long ID { get; set; }
        [Display(Name ="Ticket Number")]
        public string TicketNumber { get; set; }
        [Display(Name = "Plate Number")]
        public string Plate { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
        public bool Requested { get; private set; }
        public string RequestTime { get; private set; }
        [DataType(DataType.Time)]
        public TimeSpan? DepartureTime { get; set; }
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }
        [Display(Name = "Customer Phone")]
        public string CustomerPhone { get; set; }
    }
}