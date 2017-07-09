using EasyPark.WebUI.Areas.Manager.ViewModels.Driver;
using EasyPark.WebUI.DAL.Entities;
using System;
using System.Linq;
using System.Web.Mvc;

namespace EasyPark.WebUI.Areas.Manager.Controllers
{
    public class DriverController : Controller
    {
        private readonly ApplicationUserManager _userManager;

        public DriverController(
            ApplicationUserManager userManager
            )
        {
            _userManager = userManager;
        }

        // GET: Manager/Driver
        public ActionResult Index()
        {
            return RedirectToAction("DriverList");
        }

        public ActionResult DriverList()
        {
            var model = _userManager.Users.OfType<Driver>()
                .Select(d => new ItemViewModel
                {
                    Name = d.FirstName + " " + d.LastName,
                    Status = d.Status,
                    LastTask = d.TicketStatuses.Count == 0 ? (TicketStatusType?)null :
                        d.TicketStatuses.Where(s => s.StartDate != null && s.EndDate == null)
                        .Count() > 0 ? d.TicketStatuses.Where(s => s.StartDate != null && s.EndDate == null)
                        .FirstOrDefault().Type : d.TicketStatuses.OrderByDescending(s => s.CreationDate)
                        .FirstOrDefault().Type,
                    LastTaskStartDate = d.TicketStatuses.Count == 0 ? (DateTime?)null :
                        d.TicketStatuses.Where(s => s.StartDate != null && s.EndDate == null)
                        .Count() > 0 ? d.TicketStatuses.Where(s => s.StartDate != null && s.EndDate == null)
                        .FirstOrDefault().StartDate : d.TicketStatuses.OrderByDescending(s => s.CreationDate)
                        .FirstOrDefault().StartDate,
                    LastTaskEndDate = d.TicketStatuses.Count == 0 ? (DateTime?)null :
                        d.TicketStatuses.Where(s => s.StartDate != null && s.EndDate == null)
                        .Count() > 0 ? d.TicketStatuses.Where(s => s.StartDate != null && s.EndDate == null)
                        .FirstOrDefault().EndDate : d.TicketStatuses.OrderByDescending(s => s.CreationDate)
                        .FirstOrDefault().EndDate,
                    TransportService = d.VanID == null ? (string)null : d.Van.Name,
                    UnfinishedTaskCount = d.TicketStatuses.Where(s => s.EndDate == null).Count()
                });
            return View(model);
        }
    }
}