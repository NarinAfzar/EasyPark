using EasyPark.WebUI.Areas.ParkMan.ViewModels.Ticket;
using EasyPark.WebUI.DAL.Abstract;
using EasyPark.WebUI.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EasyPark.WebUI.Areas.ParkMan.Controllers
{
    public class TicketController : Controller
    {
        private readonly IRepository<Ticket> _ticketRepo;
        private readonly IRepository<ParkingSlot> _parkingSlotRepo;
        private readonly IRepository<TicketStatus> _ticketStatusRepo;
        private readonly ApplicationUserManager _userManager;
        private readonly IRepository<Request> _requestRepo;

        public TicketController(
            IRepository<Ticket> ticketRepo,
            IRepository<ParkingSlot> parkingSlotRepo,
            IRepository<TicketStatus> ticketStatusRepo,
            ApplicationUserManager userManager,
            IRepository<Request> requestRepo
            )
        {
            _ticketRepo = ticketRepo;
            _parkingSlotRepo = parkingSlotRepo;
            _ticketStatusRepo = ticketStatusRepo;
            _userManager = userManager;
            _requestRepo = requestRepo;
        }

        // GET: ParkMan/Ticket
        public ActionResult Index()
        {
            return RedirectToAction("DispatchedList");
        }

        public ActionResult RequestedList()
        {
            return View(_ticketRepo.GetAll()
                .Where(t => t.TicketStatuses.Any(s => s.Type == TicketStatusType.Requested && s.EndDate == null))
                .Select(t => new RequestListViewModel
                {
                    Description = t.Description,
                    ID = t.ID,
                    LicensePlate = t.LicensePlate,
                    TicketNumber = t.TicketNumber,
                    SlotNumber = t.TicketStatuses.FirstOrDefault(a => a.Type == TicketStatusType.Parked).Slot.Number
                }));
        }

        public ActionResult DispatchedList()
        {
            return View(_ticketRepo.GetAll()
                .Where(t => t.TicketStatuses.Any(s => s.Type == TicketStatusType.Dispatched && s.EndDate == null))
                .Select(t => new ListItemViewModel
                {
                    Description = t.Description,
                    ID = t.ID,
                    LicensePlate = t.LicensePlate,
                    TicketNumber = t.TicketNumber,
                    DepartureTime = t.Customer.PreferredRequestDates
                        .Where(d => d.Day == DateTime.Today.DayOfWeek).FirstOrDefault() != null
                        ? t.Customer.PreferredRequestDates
                        .Where(d => d.Day == DateTime.Today.DayOfWeek).FirstOrDefault().Time
                        : (TimeSpan?)null
                }));
        }

        public ActionResult Return(long id)
        {
            var ticket = _ticketRepo.GetAll().Where(t => t.ID == id).
                Select(t => new
                {
                    t.Description,
                    t.TicketNumber,
                    t.LicensePlate,
                    t.ID,
                    SlotNumber = t.TicketStatuses.FirstOrDefault(s => s.Type == TicketStatusType.Parked).Slot.Number
                }).FirstOrDefault();
            if (ticket == null)
            {
                return HttpNotFound();
            }

            return View(new ReturnViewModel
            {
                AvailableDrivers = GetAvailableDrivers(),
                Description = ticket.Description,
                ID = ticket.ID,
                LicensePlate = ticket.LicensePlate,
                TicketNumber = ticket.TicketNumber,
                SlotNumber = ticket.SlotNumber
            });
        }

        [HttpPost]
        public ActionResult Return(ReturnViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var returnTicket = _ticketRepo.GetAll()
                        .Where(t => t.ID == model.ID)
                        .Select(t =>
                            new
                            {
                                Ticket = t,
                                ParkedStatus = t.TicketStatuses.FirstOrDefault(s => s.Type == TicketStatusType.Parked),
                                RequestedStatus = t.TicketStatuses.FirstOrDefault(s => s.Type == TicketStatusType.Requested),
                                Slot = t.TicketStatuses
                                    .Where(s => s.Type == TicketStatusType.Parked)
                                    .Select(s => s.Slot)
                                    .FirstOrDefault()
                            })
                        .First();

                    returnTicket.ParkedStatus.EndDate = DateTime.Now;
                    if (returnTicket.RequestedStatus != null)
                    {
                        returnTicket.RequestedStatus.EndDate = DateTime.Now;
                    }
                    returnTicket.Ticket.TicketStatuses.Add(
                        new TicketStatus
                        {
                            CreationDate = DateTime.Now,
                            Type = TicketStatusType.Returning,
                            InChargeId = model.Driver
                        });

                    returnTicket.Slot.Status = returnTicket.Slot.Status == ParkingSlotStatus.Overloaded
                        ? ParkingSlotStatus.Allocated
                        : ParkingSlotStatus.Available;

                    _ticketStatusRepo.CommitChanges();
                    return RedirectToAction("RequestedList");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            else
            {
                ModelState.AddModelError("", "invalid data");
            }

            var ticket = _ticketRepo.GetAll().Where(t => t.ID == model.ID).
                Select(t => new
                {
                    t.Description,
                    t.TicketNumber,
                    t.LicensePlate,
                    t.ID,
                    SlotNumber = t.TicketStatuses.FirstOrDefault(s => s.Type == TicketStatusType.Parked).Slot.Number
                }).FirstOrDefault();

            model.AvailableDrivers = GetAvailableDrivers();
            model.Description = ticket.Description;
            model.ID = ticket.ID;
            model.LicensePlate = ticket.LicensePlate;
            model.TicketNumber = ticket.TicketNumber;
            model.SlotNumber = ticket.SlotNumber;

            return View(model);
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

        public ActionResult Park(long id)
        {
            return View(CreateParkViewModel(id));
        }

        [HttpPost]
        public ActionResult Park(ParkViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var slot = _parkingSlotRepo.GetAll().Where(s => s.ID == model.Slot).First();
                    if (!(slot.Status == ParkingSlotStatus.Overloaded || slot.Status == ParkingSlotStatus.Disabled))
                    {
                        slot.Status = slot.Status == ParkingSlotStatus.Allocated
                            ? ParkingSlotStatus.Overloaded
                            : ParkingSlotStatus.Allocated;

                        _ticketStatusRepo.GetAll()
                            .First(s =>
                                s.TicketID == model.ID && s.Type == TicketStatusType.Dispatched).EndDate = DateTime.Now;

                        _ticketStatusRepo.InsertOnCommit(new TicketStatus
                        {
                            CreationDate = DateTime.Now,
                            StartDate = DateTime.Now,
                            SlotId = model.Slot,
                            TicketID = model.ID,
                            Type = TicketStatusType.Parked
                        });
                        _ticketStatusRepo.CommitChanges();

                        return RedirectToAction("DispatchedList");
                    }
                }
                catch
                {
                    ModelState.AddModelError("", "Can not park in selected slot.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid data.");
            }

            return View(CreateParkViewModel(model.ID));
        }

        private ParkViewModel CreateParkViewModel(long ticketId)
        {
            var ticket = _ticketRepo.GetAll().Where(t => t.ID == ticketId)
                .Select(t => new
                {
                    t.Description,
                    t.ID,
                    t.LicensePlate,
                    t.TicketNumber
                }).FirstOrDefault();
            return new ParkViewModel
            {
                AvailableSolts = _parkingSlotRepo.GetAll()
                .Where(s => s.Status == ParkingSlotStatus.Allocated || s.Status == ParkingSlotStatus.Available)
                .OrderBy(s => s.Status).ThenBy(s => s.Number)
                .Select(s => new SelectListItem
                {
                    Value = s.ID.ToString(),
                    Text = s.Number + (s.Status == ParkingSlotStatus.Allocated ? "(*)" : "")
                }),
                Description = ticket.Description,
                ID = ticket.ID,
                LicensePlate = ticket.LicensePlate,
                TicketNumber = ticket.TicketNumber,
            };
        }

        public JsonResult CheckRequestedCar()
        {
            var requests = _ticketRepo.GetAll()
                .Where(t => t.TicketStatuses.Any(s => s.Type == TicketStatusType.Requested && s.EndDate == null)).Count();
            if (requests > 0)
            {
                return Json(requests, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("0", JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetDispatchedCarsCount()
        {
            var count = _ticketRepo.GetAll()
                .Where(t => t.TicketStatuses.Any(s => s.Type == TicketStatusType.Dispatched && s.EndDate == null)).Count();
            if (count > 0)
            {
                return Json(count, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ParkedList()
        {
            return View(_ticketRepo.GetAll()
                .Where(t => t.TicketStatuses.Any(s => s.EndDate == null && s.Type == TicketStatusType.Parked))
                .Select(t => new ParkedListViewModel
                {
                    ID = t.ID,
                    Description = t.Description,
                    Plate = t.LicensePlate,
                    DepartureTime = t.Customer.PreferredRequestDates
                        .Where(d => d.Day == DateTime.Today.DayOfWeek).Select(p=>p.Time).FirstOrDefault(),
                    TicketNumber = t.TicketNumber,
                    ParkDate = t.TicketStatuses
                    .FirstOrDefault(s => s.EndDate == null && s.Type == TicketStatusType.Parked).StartDate.Value,
                    ParkingSlot = t.TicketStatuses
                    .FirstOrDefault(s => s.EndDate == null && s.Type == TicketStatusType.Parked).Slot.Number,
                    RequestTime = t.Requests.Count == 0 ? (DateTime?)null : t.Requests.FirstOrDefault().RequestDate
                }));
        }
    }
}