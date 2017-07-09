using EasyPark.WebUI.Areas.Manager.ViewModels.Ticket;
using EasyPark.WebUI.DAL.Abstract;
using EasyPark.WebUI.DAL.Entities;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EasyPark.WebUI.Areas.Manager.Controllers
{
    public class TicketController : Controller
    {
        private readonly ApplicationUserManager _userManager;
        private readonly IRepository<Ticket> _ticketRepo;
        private readonly IRepository<Request> _requestRepo;
        private readonly IRepository<TicketStatus> _ticketStatusRepo;

        public TicketController(
            IRepository<Ticket> ticketRepo,
            IRepository<Request> requestRepo,
            IRepository<TicketStatus> ticketStatusRepo,
            ApplicationUserManager userManager)
        {
            _ticketRepo = ticketRepo;
            _requestRepo = requestRepo;
            _ticketStatusRepo = ticketStatusRepo;
            _userManager = userManager;
        }

        public ActionResult Index()
        {
            return View(_ticketStatusRepo.GetAll()
                .Where(t => t.EndDate == null)
                .Select(t => new ActiveTicketViewModel
                {
                    ID = t.TicketID,
                    Plate = t.Ticket.LicensePlate,
                    TicketNumber = t.Ticket.TicketNumber,
                    Status = t.Type,
                    CustomerName=t.Ticket.Customer.FirstName+" "+t.Ticket.Customer.LastName,
                    CustomerPhone=t.Ticket.Customer.UserName
                }));
        }

        public ActionResult Create(bool status = false)
        {
            return View(new CreateViewModel
            {
                Status = status,
                Drivers = GetAvailableDrivers()
            });
        }

        [HttpPost]
        public ActionResult Create(CreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Get customer information
                    var customer = _userManager.Users.Where(u => u.UserName == model.PhoneNumber).FirstOrDefault();
                    if (customer == null)
                    {
                        customer = new ApplicationUser
                        {
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            PhoneNumber = model.PhoneNumber,
                            UserName = model.PhoneNumber
                        };
                        if (!_userManager.Create(customer, "123456").Succeeded)
                        {
                            ModelState.AddModelError("", "Failed to save customer information.");
                            model.Drivers = GetAvailableDrivers();
                            return View(model);
                        }
                    }

                    // Updating TimeOfDay Information
                    SetPreferedTimeForCustomer(customer, model.DepartureTime);

                    // Create ticket
                    Ticket newTicket = new Ticket
                    {
                        CustomerID = customer.Id,
                        Description = "No Desc",
                        LicensePlate = model.Plate,
                        TicketNumber = model.TicketNumber,
                        Color = model.Color,
                        TicketStatuses = new List<TicketStatus>
                        {
                            new TicketStatus
                            {
                                 CreationDate=DateTime.Now,
                                 StartDate=DateTime.Now,
                                 EndDate=DateTime.Now,
                                 Slot=null,
                                 Type=TicketStatusType.CheckedOut
                            }
                        }
                    };

                    if (model.Driver != null)
                    {
                        newTicket.TicketStatuses.Add(new TicketStatus
                        {
                            CreationDate = DateTime.Now,
                            InChargeId = model.Driver,
                            Slot = null,
                            Type = TicketStatusType.Dispatching
                        });
                    }

                    _ticketRepo.InsertOnCommit(newTicket);
                    _ticketRepo.CommitChanges();
                    return RedirectToAction("CreateNew");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid data.");
            }
            model.Drivers = GetAvailableDrivers();
            return View(model);
        }

        private void SetPreferedTimeForCustomer(ApplicationUser customer, TimeSpan departureTime)
        {
            if (customer.PreferredRequestDates == null)
            {
                customer.PreferredRequestDates = new List<TimeOfDay>();
            }
            var preferedTime = customer.PreferredRequestDates.Where(p => p.Day == DateTime.Today.DayOfWeek).FirstOrDefault();
            if (preferedTime == null)
            {
                customer.PreferredRequestDates.Add(new TimeOfDay
                {
                    Day = DateTime.Today.DayOfWeek,
                    Time = departureTime
                });
            }
            else
            {
                preferedTime.Time = departureTime;
            }
            _ticketRepo.CommitChanges();
        }

        private IEnumerable<SelectListItem> GetAvailableDrivers()
        {
            return _userManager.Users.OfType<Driver>()
                .Where(d => d.Status == DriverStatusType.Available)
                .Where(d => d.TicketStatuses.Where(s => s.EndDate == null).Count() < 2)
                .Select(d => new
                {
                    Driver = d,
                    TaskCount = d.TicketStatuses.Where(s => s.EndDate == null).Count()
                })
                .OrderBy(d => d.TaskCount)
                .Select(d => new SelectListItem
                {
                    Value = d.Driver.Id,
                    Text = d.Driver.FirstName + " " + d.Driver.LastName + "(" + d.TaskCount + ") - " +
                        (d.Driver.TicketStatuses.Any(s => s.StartDate != null && s.EndDate == null)
                            ? "Driving"
                            : d.Driver.TicketStatuses.Where(s => s.EndDate == null).Count() == 0
                                && d.Driver.TicketStatuses.OrderByDescending(s => s.ID)
                                .FirstOrDefault().Type == TicketStatusType.Dispatching
                                ? "In parking"
                                : d.Driver.TicketStatuses.Where(s => s.EndDate == null).Count() == 0
                                    && d.Driver.TicketStatuses.OrderByDescending(s => s.ID)
                                    .FirstOrDefault().Type == TicketStatusType.Returning
                                    ? "In Gate"
                                    : "Available")
                });
        }

        public ActionResult CreateNew()
        {
            return RedirectToAction("Create", new { status = true });
        }

        public JsonResult CheckPlate(string plate)
        {
            try
            {
                return Json(_ticketRepo.GetAll().Where(t => t.LicensePlate == plate)
                    .Select(t => new TicketViewModel
                    {
                        Color = t.Color,
                        PhoneNumber = t.Customer.PhoneNumber,
                        FirstName = t.Customer.FirstName,
                        LastName = t.Customer.LastName,
                        SetDepartureTime = t.Customer.PreferredRequestDates
                            .Where(p => p.Day == DateTime.Today.DayOfWeek)
                            .FirstOrDefault() == null ? default(TimeSpan) : t.Customer.PreferredRequestDates
                            .Where(p => p.Day == DateTime.Today.DayOfWeek)
                            .FirstOrDefault().Time
                    }).First(), JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("0", JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult CheckPhoneNumber(string phoneNumber)
        {
            try
            {
                return Json(_userManager.Users.Where(a => a.PhoneNumber == phoneNumber)
                    .Select(t => new TicketViewModel
                    {
                        FirstName = t.FirstName,
                        LastName = t.LastName,
                    }).First(), JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("0", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ParkedList()
        {
            return View(
                _ticketRepo.GetAll()
                .Where(t => t.TicketStatuses.OrderByDescending(s => s.ID).FirstOrDefault().Type == TicketStatusType.Parked)
                .Select(t => new ParkedListViewModel
                {
                    ID = t.ID,
                    Description = t.Description,
                    Plate = t.LicensePlate,
                    TicketNumber = t.TicketNumber,
                    SetRequest = t.Requests.Count > 0 ? t.Requests.FirstOrDefault().RequestDate : (DateTime?)null,
                    DepartureTime = t.Customer.PreferredRequestDates
                        .Where(d => d.Day == DateTime.Today.DayOfWeek).FirstOrDefault().Time,
                    CustomerName = t.Customer.FirstName + " " + t.Customer.LastName,
                    CustomerPhone = t.Customer.UserName
                }));
        }

        public ActionResult CarRequest(long id)
        {
            var ticket = _ticketRepo.GetAll().Where(c => c.ID == id).FirstOrDefault();
            if (ticket != null)
            {
                ticket.TicketStatuses.Add(new TicketStatus
                {
                    CreationDate = DateTime.Now,
                    StartDate = DateTime.Now,
                    Type = TicketStatusType.Requested
                });
                foreach (var item in ticket.Requests)
                {
                    item.Read = true;
                }
                _ticketRepo.CommitChanges();
            }
            return RedirectToAction("ParkedList");
        }

        public JsonResult CheckRequestedCar()
        {
            var requests = _requestRepo.GetAll()
                .Where(c => c.Read == false).Count();
            if (requests > 0)
            {
                return Json(requests, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("0", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CheckedOutList()
        {
            return View(_ticketRepo.GetAll()
                .Where(t => t.TicketStatuses.OrderByDescending(s => s.ID)
                .FirstOrDefault().Type == TicketStatusType.CheckedOut)
                .Select(t => new ReturnListViewModel
                {
                    Description = t.Description,
                    ID = t.ID,
                    LicensePlate = t.LicensePlate,
                    TicketNumber = t.TicketNumber
                }));
        }

        public ActionResult AssignDriver(long id)
        {
            return View(new AssignDriverViewModel
            {
                Drivers = GetAvailableDrivers(),
                TicketId = id
            });
        }

        [HttpPost]
        public ActionResult AssignDriver(AssignDriverViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _ticketStatusRepo.InsertOnCommit(new TicketStatus
                    {
                        CreationDate = DateTime.Now,
                        InChargeId = model.DriverId,
                        Slot = null,
                        Type = TicketStatusType.Dispatching,
                        TicketID = model.TicketId
                    });
                    _ticketRepo.CommitChanges();
                    return RedirectToAction("CheckedOutList");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid data.");
            }
            model.Drivers = GetAvailableDrivers();
            return View(model);
        }

        public ActionResult CheckedInList()
        {
            return View(_ticketRepo.GetAll()
                .Where(t => t.TicketStatuses.Any(s => s.Type == TicketStatusType.CheckedIn && s.EndDate == null))
                .Select(t => new ReturnListViewModel
                {
                    Description = t.Description,
                    ID = t.ID,
                    LicensePlate = t.LicensePlate,
                    TicketNumber = t.TicketNumber,
                    CustomerName = t.Customer.FirstName + " " + t.Customer.LastName,
                    CustomerPhone = t.Customer.UserName
                }));
        }

        public JsonResult GetCheckedInCarsCount()
        {
            var count = _ticketRepo.GetAll()
                .Where(t => t.TicketStatuses.Any(s => s.Type == TicketStatusType.CheckedIn && s.EndDate == null)).Count();
            if (count > 0)
            {
                return Json(count, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ReturnCarToCustomer(long id)
        {
            try
            {
                _ticketStatusRepo.GetAll()
                    .FirstOrDefault(s => s.TicketID == id && s.Type == TicketStatusType.CheckedIn).EndDate = DateTime.Now;
                _ticketStatusRepo.CommitChanges();
            }
            catch
            {
            }
            return RedirectToAction("CheckedInList");
        }
    }
}