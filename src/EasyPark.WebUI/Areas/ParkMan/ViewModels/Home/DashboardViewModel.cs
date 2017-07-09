using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyPark.WebUI.Areas.ParkMan.ViewModels.Home
{
    public class DashboardViewModel
    {
        public int ParkedCarCount { get; set; }
        public int DispathedCarCount { get; set; }
    }
}