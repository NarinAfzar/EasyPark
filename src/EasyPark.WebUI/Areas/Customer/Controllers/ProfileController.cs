using EasyPark.WebUI.Areas.Customer.ViewModels.Profile;
using EasyPark.WebUI.DAL.Abstract;
using EasyPark.WebUI.DAL.Entities;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace EasyPark.WebUI.Areas.Customer.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApplicationUserManager _userManager;
        private readonly IRepository<Ticket> _ticketRepo;
        // GET: Customer/Profile
        public ActionResult Index()
        {
            return RedirectToAction("Edit");
        }

        public ProfileController(
            ApplicationUserManager userManager,
            IRepository<Ticket> ticketRepo)
        {
            _ticketRepo = ticketRepo;
            _userManager = userManager;
        }


        public ActionResult Edit()
        {
            var customerID = User.Identity.GetUserId();
            return View(_userManager.Users
                .Where(u => u.Id == customerID)
                .Select(u => new ProfileSettingsViewModel
                {
                    Name = u.FirstName,
                    Family = u.LastName,
                    UserName = u.UserName,
                }).FirstOrDefault());
        }

        [HttpPost]
        public ActionResult Edit(ProfileSettingsViewModel model)
        {
            var customerID = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                try
                {
                    var customer = _userManager.Users.Where(u => u.Id == customerID).First();
                    customer.FirstName = model.Name;
                    customer.LastName = model.Family;
                    _ticketRepo.CommitChanges();
                }
                catch
                {
                    ModelState.AddModelError("", "Update failed.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid data.");
            }
            return View(_userManager.Users
                .Where(u => u.Id == customerID)
                .Select(u => new ProfileSettingsViewModel
                {
                    Name = u.FirstName,
                    Family = u.LastName,
                    UserName = u.UserName,
                }).FirstOrDefault());
        }

        private PreferedTimesViewModel[] GeneratePreferedTimesForUser()
        {
            var customerID = User.Identity.GetUserId();
            return _userManager.Users
                .Where(u => u.Id == customerID)
                .SelectMany(u => u.PreferredRequestDates)
                .Select(p => new PreferedTimesViewModel
                {
                    Day = p.Day,
                    DepartureTime = p.Time
                }).ToList()
                .Concat(new[]
                {
                    new PreferedTimesViewModel { Day=DayOfWeek.Friday},
                    new PreferedTimesViewModel { Day=DayOfWeek.Monday},
                    new PreferedTimesViewModel { Day=DayOfWeek.Saturday},
                    new PreferedTimesViewModel { Day=DayOfWeek.Sunday},
                    new PreferedTimesViewModel { Day=DayOfWeek.Thursday},
                    new PreferedTimesViewModel { Day=DayOfWeek.Tuesday},
                    new PreferedTimesViewModel { Day=DayOfWeek.Wednesday}
                })
                .GroupBy(p => p.Day)
                .Select(g => new PreferedTimesViewModel
                {
                    Day = g.Key,
                    DepartureTime = g.Where(p => p.DepartureTime != default(TimeSpan)).FirstOrDefault() == null
                        ? default(TimeSpan)
                        : g.Where(p => p.DepartureTime != default(TimeSpan)).FirstOrDefault().DepartureTime
                })
                .OrderBy(p => p.Day)
                .ToArray();
        }

        public ActionResult PreferedTimes()
        {
            return View(GeneratePreferedTimesForUser());
        }

        [HttpPost]
        public ActionResult PreferedTimes(IEnumerable<PreferedTimesViewModel> model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var customerID = User.Identity.GetUserId();
                    var user = _userManager.Users.Where(u => u.Id == customerID).Include(x => x.PreferredRequestDates).First();
                    user.PreferredRequestDates.Clear();
                    foreach (var item in model)
                    {
                        if (item.DepartureTime != null)
                        {
                            user.PreferredRequestDates.Add(new TimeOfDay
                            {
                                Day = item.Day,
                                Time = item.DepartureTime.Value
                            });
                        }
                    }
                    _ticketRepo.CommitChanges();
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
            return View(GeneratePreferedTimesForUser());
        }

    }
}