using EasyPark.WebUI.Areas.Manager.ViewModels.Home;
using EasyPark.WebUI.DAL.Entities;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using EasyPark.WebUI.DAL.Abstract;

namespace EasyPark.WebUI.Areas.Manager.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationUserManager _userManager;
        private readonly IRepository<TicketStatus> _ticketStatusRepo;
            
        public HomeController(ApplicationUserManager userManager, IRepository<TicketStatus> ticketStatusRepo)
        {
            _userManager = userManager;
            _ticketStatusRepo = ticketStatusRepo;
        }

        // GET: Manager/Home
        public ActionResult Index()
        {
            return RedirectToAction("Create", "Ticket");
        }

        public ActionResult Dashboard()
        {
            return View(
                new DashboardViewModel
                {
                    DriverCount = GetAvailableDriversCount(),
                    TicketCount = GetActiveTicketCount()
                });
        }

        private int GetAvailableDriversCount()
        {
            return _userManager.Users.OfType<Driver>()
                .Where(d => d.Status == DriverStatusType.Available)
                .Where(d => d.TicketStatuses.Where(s => s.EndDate == null).Count() < 2)
                .Count();
        }

        private int GetActiveTicketCount()
        {
            return _ticketStatusRepo.GetAll()
                .Where(t => t.EndDate == null).Count();
        }

    }
}