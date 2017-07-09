using EasyPark.WebUI.DAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EasyPark.WebUI.Areas.CarDriver.ViewModels
{
    public class VanSwipeViewModel
    {
        [Display(Name = "Driver:")]
        public IEnumerable<SelectListItem> Drivers { set; get; }
        [Display(Name ="Van:")]
        public IEnumerable<SelectListItem> Vans { set; get; }
        public string DriverId { get; set; }
        public long VanId { get; set; }
    }
}