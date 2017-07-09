using System.Web.Mvc;

namespace EasyPark.WebUI.Areas.ParkMan
{
    public class ParkManAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "ParkMan";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "ParkMan_default",
                "ParkMan/{controller}/{action}/{id}",
                new { action = "Index", controller = "Home", id = UrlParameter.Optional }
            );
        }
    }
}