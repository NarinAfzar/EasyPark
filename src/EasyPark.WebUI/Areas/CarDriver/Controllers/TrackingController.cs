using EasyPark.WebUI.Areas.CarDriver.ViewModels;
using EasyPark.WebUI.DAL.Abstract;
using EasyPark.WebUI.DAL.Entities;
using System;
using System.Linq;
using System.Web.Mvc;

namespace EasyPark.WebUI.Areas.CarDriver.Controllers
{
    public class TrackingController : Controller
    {
        private readonly IRepository<Van> _vanRepo;
        private readonly IRepository<TicketStatus> _ticketStatusRepo;
        private readonly IRepository<Ticket> _ticketRepo;
        private readonly ApplicationUserManager _userManager;

        public TrackingController(
            IRepository<Van> vanRepo,
            IRepository<Ticket> ticketRepo,
            IRepository<TicketStatus> ticketStatusRepo,
            ApplicationUserManager userManager
            )
        {
            _vanRepo = vanRepo;
            _ticketRepo = ticketRepo;
            _ticketStatusRepo = ticketStatusRepo;
            _userManager = userManager;
        }

        // GET: CarDriver/Status
        public ActionResult Index()
        {
            return RedirectToAction("SwipeInParking");
        }

        public ActionResult SwipeInVan()
        {
            return View(new VanSwipeViewModel
            {
                Drivers= _userManager.Users.OfType<Driver>()
                .Select(c => new SelectListItem
                {
                    Text = c.FirstName + " " + c.LastName,
                    Value = c.Id.ToString()
                }),
                Vans= _vanRepo.GetAll().Select(v => new SelectListItem
                {
                    Text = v.Name,
                    Value = v.ID.ToString()
                })
            });
        }

        [HttpPost]
        public ActionResult SwipeInVan(VanSwipeViewModel model)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    var driver = _userManager.Users.OfType<Driver>().Where(d => d.Id == model.DriverId).First();
                    driver.VanID = driver.VanID == null ? model.VanId : (long?)null;
                    _vanRepo.CommitChanges();

                }
                catch
                {
                    ModelState.AddModelError("", "Can not find the driver specified.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid data.");
            }
            model.Drivers = _userManager.Users.OfType<Driver>()
                .Select(c => new SelectListItem
                {
                    Text = c.FirstName + " " + c.LastName,
                    Value = c.Id.ToString()
                });
            model.Vans = _vanRepo.GetAll()
                .Select(v => new SelectListItem
                {
                    Text = v.Name,
                    Value = v.ID.ToString()
                });

            return View(model);
        }

        public ActionResult SwipeInParking()
        {
            return View(new SwipeViewModel
            {
                Drivers = _userManager.Users.OfType<Driver>()
                    .Select(c => new SelectListItem
                    {
                        Text = c.FirstName + " " + c.LastName,
                        Value = c.Id.ToString()
                    })
            });
        }

        [HttpPost]
        public ActionResult SwipeInParking(SwipeViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool finishTaskStatus = false;
                try
                {
                    finishTaskStatus = DriverSwipeReflector(model.Driver, SwipeState.InParking);
                }
                catch
                {
                }
                return RedirectToAction("TaskList", new { driverID = model.Driver, finished = finishTaskStatus });
            }
            model.Drivers = _userManager.Users.OfType<Driver>()
                .Select(c => new SelectListItem
                {
                    Text = c.FirstName + " " + c.LastName,
                    Value = c.Id
                });
            return View(model);
        }

        public ActionResult TaskList(string driverID, bool finished = false)
        {
            return View(_userManager.Users.OfType<Driver>()
                .Where(d => d.Id == driverID)
                .Select(d => new TaskListViewModel
                {
                    Current = d.TicketStatuses.Where(s => s.EndDate == null && s.StartDate != null)
                    .Select(f => new TaskViewModel
                    {
                        CreationDate = f.CreationDate,
                        TicketID = f.TicketID,
                        Type = f.Type.ToString(),
                        StatusId = f.ID
                    }).FirstOrDefault(),
                    Assigned = d.TicketStatuses.Where(s => s.EndDate == null && s.StartDate == null)
                    .Select(f => new TaskViewModel
                    {
                        CreationDate = f.CreationDate,
                        TicketID = f.TicketID,
                        Type = f.Type.ToString(),
                        StatusId = f.ID
                    }),
                    Finished = finished==false?null
                    : d.TicketStatuses.Where(s => s.EndDate != null).OrderByDescending(s=>s.EndDate)
                    .Select(f => new TaskViewModel
                    {
                        CreationDate = f.CreationDate,
                        TicketID = f.TicketID,
                        Type = f.Type.ToString(),
                        StatusId = f.ID
                    }).FirstOrDefault()
                }).FirstOrDefault());
        }

        public ActionResult SwipeInGate()
        {
            return View(new SwipeViewModel
            {
                Drivers = _userManager.Users.OfType<Driver>()
                    .Select(c => new SelectListItem
                    {
                        Text = c.FirstName + " " + c.LastName,
                        Value = c.Id
                    })
            });
        }

        public ActionResult CancelCurrentTask(long Id)
        {
            var task = _ticketStatusRepo.GetAll().Where(s => s.ID == Id).FirstOrDefault();
            if (task != null)
            {
                if (task.Type == TicketStatusType.Dispatching || task.Type == TicketStatusType.Returning)
                {
                    if (!string.IsNullOrEmpty(task.InChargeId))
                    {
                        task.StartDate = null;
                        _ticketStatusRepo.CommitChanges();
                    }
                }
            }
            return RedirectToAction("TaskList", new { driverId = task.InChargeId });
        }

        [HttpPost]
        public ActionResult SwipeInGate(SwipeViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool finishTaskStatus = false;
                try
                {
                    finishTaskStatus = DriverSwipeReflector(model.Driver, SwipeState.InGate);
                }
                catch
                {
                }
                return RedirectToAction("TaskList", new { driverID = model.Driver, finished = finishTaskStatus });
            }
            model.Drivers = _userManager.Users.OfType<Driver>()
                .Select(c => new SelectListItem
                {
                    Text = c.FirstName + " " + c.LastName,
                    Value = c.Id
                });
            return View(model);
        }

        private bool DriverSwipeReflector(string driverId, SwipeState state)
        {
            var driver = _userManager.Users.OfType<Driver>().Where(d => d.Id == driverId).FirstOrDefault();
            if (driver == null)
            {
                throw new ArgumentException("Driver does not exist.");
            }

            if (driver.VanID != null)
            {
                driver.VanID = null;
                _vanRepo.CommitChanges();
            }

            if (driver.Status == DriverStatusType.Off)
            {
                throw new InvalidOperationException("Driver is off duty.");
            }

            var startTask = GetStartTask(driver, state);
            var endTask = GetEndTask(driver, state);

            if (endTask != null)
            {
                CheckIn(driver, state);
                return true;
            }
            else if (startTask != null)
            {
                CheckOut(driver, state);
                return false;
            }
            throw new InvalidOperationException("There are no tasks to start/finish in this station for this driver.");
        }

        private void CheckOut(Driver driver, SwipeState state)
        {
            if (driver.TicketStatuses.Any(s => s.StartDate != null && s.EndDate == null))
            {
                throw new InvalidOperationException("Driver has been already assigned a task.");
            }
            switch (state)
            {
                case SwipeState.InGate:
                    driver.TicketStatuses
                        .Where(s => s.StartDate == null && s.Type == TicketStatusType.Dispatching)
                        .OrderBy(s => s.CreationDate).FirstOrDefault().StartDate = DateTime.Now;
                    break;

                case SwipeState.InParking:
                    driver.TicketStatuses
                        .Where(s => s.StartDate == null && s.Type == TicketStatusType.Returning)
                        .OrderBy(s => s.CreationDate).FirstOrDefault().StartDate = DateTime.Now;
                    break;
            }
            _ticketStatusRepo.CommitChanges();
        }

        private void CheckIn(Driver driver, SwipeState state)
        {
            switch (state)
            {
                case SwipeState.InGate:
                    var inGateFinishTask = driver.TicketStatuses
                        .Where(s => s.StartDate != null && s.EndDate == null && s.Type == TicketStatusType.Returning)
                        .OrderBy(s => s.CreationDate).FirstOrDefault();
                    inGateFinishTask.EndDate = DateTime.Now;
                    inGateFinishTask.Ticket.TicketStatuses.Add(new TicketStatus
                    {
                        CreationDate = DateTime.Now,
                        Type = TicketStatusType.CheckedIn,
                        StartDate = DateTime.Now,
                        TicketID = inGateFinishTask.Ticket.ID
                    });
                    break;

                case SwipeState.InParking:
                    var inParkingFinishTask = driver.TicketStatuses
                        .Where(s => s.StartDate != null && s.EndDate == null && s.Type == TicketStatusType.Dispatching)
                        .OrderBy(s => s.CreationDate).FirstOrDefault();
                    inParkingFinishTask.EndDate = DateTime.Now;
                    inParkingFinishTask.Ticket.TicketStatuses.Add(new TicketStatus
                    {
                        CreationDate = DateTime.Now,
                        Type = TicketStatusType.Dispatched,
                        StartDate = DateTime.Now,
                        TicketID = inParkingFinishTask.Ticket.ID
                    });
                    break;
            }
            _ticketStatusRepo.CommitChanges();
        }

        private TicketStatus GetStartTask(Driver driver, SwipeState state)
        {
            switch (state)
            {
                case SwipeState.InGate:
                    return driver.TicketStatuses
                        .Where(s => s.StartDate == null && s.Type == TicketStatusType.Dispatching)
                        .OrderBy(s => s.CreationDate).FirstOrDefault();

                case SwipeState.InParking:
                    return driver.TicketStatuses
                        .Where(s => s.StartDate == null && s.Type == TicketStatusType.Returning)
                        .OrderBy(s => s.CreationDate).FirstOrDefault();
            }
            return null;
        }

        private TicketStatus GetEndTask(Driver driver, SwipeState state)
        {
            switch (state)
            {
                case SwipeState.InGate:
                    return driver.TicketStatuses
                        .Where(s => s.StartDate != null && s.EndDate == null && s.Type == TicketStatusType.Returning)
                        .FirstOrDefault();

                case SwipeState.InParking:
                    return driver.TicketStatuses
                        .Where(s => s.StartDate != null && s.EndDate == null && s.Type == TicketStatusType.Dispatching)
                        .FirstOrDefault();
            }
            return null;
        }
    }
}