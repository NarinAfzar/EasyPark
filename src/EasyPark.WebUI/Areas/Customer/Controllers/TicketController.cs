using EasyPark.WebUI.DAL.Abstract;
using EasyPark.WebUI.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using EasyPark.WebUI.Areas.Customer.ViewModels.Ticket;

namespace EasyPark.WebUI.Areas.Customer.Controllers
{
    [Authorize]
    public class TicketController : Controller
    {
        private readonly IRepository<TicketStatus> _ticketStatusRepo;
        private readonly IRepository<Request> _requestRepo;

        public TicketController(
            IRepository<TicketStatus> ticketStatusRepo,
            IRepository<Request> requestRepo)
        {
            _ticketStatusRepo = ticketStatusRepo;
            _requestRepo = requestRepo;
        }

        public ActionResult Index()
        {
            return RedirectToAction("ParkedList");
        }

        public ActionResult ParkedList()
        {
            var customerID = User.Identity.GetUserId();
            return View(_ticketStatusRepo.GetAll()
                .Where(s => s.Type != TicketStatusType.Requested)
                .Where(s => s.Ticket.CustomerID == customerID)
                .Where(s => s.EndDate == null)
                .Select(s => new ParkedListViewModel
                {
                    DepartureDate = s.Ticket.Customer.PreferredRequestDates
                        .Where(p => p.Day == DateTime.Today.DayOfWeek).FirstOrDefault() != null
                        ? s.Ticket.Customer.PreferredRequestDates
                        .Where(p => p.Day == DateTime.Today.DayOfWeek).FirstOrDefault().Time
                        : (TimeSpan?)null,
                    ID = s.Ticket.ID,
                    LicensePlate = s.Ticket.LicensePlate,
                    TicketNumber = s.Ticket.TicketNumber,
                    IsRequested = s.Ticket.Requests.Count > 0 ? true : false
                }));
        }

        public ActionResult SendRequest(long id, int time)
        {
            try
            {
                _requestRepo.InsertOnCommit(new Request
                {
                    TicketId = id,
                    RequestDate = DateTime.Now + TimeSpan.FromMinutes(time),
                    Read = false
                });
                _requestRepo.CommitChanges();

            }
            catch
            {
            }
            return RedirectToAction("ParkedList");
        }

    }
}