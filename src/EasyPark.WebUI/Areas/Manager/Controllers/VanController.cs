using EasyPark.WebUI.DAL.Abstract;
using EasyPark.WebUI.DAL.Entities;
using System.Drawing;
using System.Linq;
using System.Web.Mvc;

namespace EasyPark.WebUI.Areas.Manager.Controllers
{
    public class VanController : Controller
    {
        private readonly IRepository<Van> _vanRepo;

        public VanController(IRepository<Van> vanRepo)
        {
            _vanRepo = vanRepo;
        }

        public ActionResult Index()
        {
            return RedirectToAction("Track");
        }

        public ActionResult Track()
        {
            return View();
        }

        public ActionResult Vans()
        {
            return Json(_vanRepo.GetAll().ToList().Select(v =>
                new
                {
                    ID = v.ID,
                    Color = Color.FromArgb(v.Color),
                    Name = v.Name,
                    Drivers = v.Drivers.Select(d => d.FirstName + " " + d.LastName)
                }), JsonRequestBehavior.AllowGet);
        }
    }
}