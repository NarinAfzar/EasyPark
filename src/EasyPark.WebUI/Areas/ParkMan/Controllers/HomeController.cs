using EasyPark.WebUI.Areas.ParkMan.ViewModels.Home;
using EasyPark.WebUI.DAL.Abstract;
using EasyPark.WebUI.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EasyPark.WebUI.Areas.ParkMan.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository<Ticket> _ticketRepo;
            
        public HomeController(IRepository<Ticket> ticketRepo)
        {
            _ticketRepo = ticketRepo;
        }
        // GET: ParkMan/Home
        public ActionResult Index()
        {
            return RedirectToAction("Dashboard");
        }

        public ActionResult Dashboard()
        {
            return View(
                new DashboardViewModel
                {
                    DispathedCarCount= GetDispachedCarCount(),
                    ParkedCarCount= GetParkedCarCount()
                });
        }

        private int GetDispachedCarCount()
        {
            return (_ticketRepo.GetAll()
                .Where(a => a.TicketStatuses.Any(l => l.Type == TicketStatusType.Dispatched && l.EndDate == null))
                .Count()
                );
        }
        private int GetParkedCarCount()
        {
            return (_ticketRepo.GetAll()
                .Where(a => a.TicketStatuses.Any(l => l.Type == TicketStatusType.Parked && l.EndDate == null))
                .Count()
                );
        }
    }
}