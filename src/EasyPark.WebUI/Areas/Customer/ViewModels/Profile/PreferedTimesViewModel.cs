using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EasyPark.WebUI.Areas.Customer.ViewModels.Profile
{
    public class PreferedTimesViewModel
    {
        public DayOfWeek Day { set; get; }
        [DataType(DataType.Time)]
        public TimeSpan? DepartureTime { set; get; }
    }
}