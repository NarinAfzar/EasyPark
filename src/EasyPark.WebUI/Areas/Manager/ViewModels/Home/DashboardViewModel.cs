using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EasyPark.WebUI.Areas.Manager.ViewModels.Home
{
    public class DashboardViewModel
    {
        public int DriverCount { get; set; }

        public int TicketCount { get; set; }              
    }
}