using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EasyPark.WebUI.Areas.Customer.ViewModels.Profile
{
    public class ProfileSettingsViewModel
    {
        [Display(Name ="First name")]
        public string Name { set; get; }
        [Display(Name = "Last name")]
        public string Family { set; get; }
        [Display(Name = "User name/Phone number")]
        public string UserName { set; get; }

    }
}