using System.Collections.Generic;
using System.Web.Mvc;

namespace EasyPark.WebUI.Areas.CarDriver.ViewModels
{
    public class SwipeViewModel
    {
        public string Driver { get; set; }

        public IEnumerable<SelectListItem> Drivers { get; set; }
    }
}