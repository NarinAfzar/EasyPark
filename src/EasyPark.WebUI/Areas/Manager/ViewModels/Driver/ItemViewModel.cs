using EasyPark.WebUI.DAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EasyPark.WebUI.Areas.Manager.ViewModels.Driver
{
    public class ItemViewModel
    {
        [Display(Name = "Driver name")]
        public string Name { get; set; }
        [Display(Name = "Assigned tasks")]
        public int UnfinishedTaskCount { get; set; }
        [Display(Name = "Status")]
        public DriverStatusType Status { get; set; }
        [Display(Name = "Last task")]
        public TicketStatusType? LastTask { get; set; }
        [Display(Name = "Last task start date")]
        public DateTime? LastTaskStartDate { set; get; }
        [Display(Name = "Last task end date")]
        public DateTime? LastTaskEndDate { set; get; }
        [Display(Name = "Transport service")]
        public string TransportService { set; get; }
    }
}