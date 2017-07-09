using EasyPark.WebUI.DAL.Entities;
using EasyPark.WebUI.Domain.Models;
using EasyPark.WebUI.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;

namespace EasyPark.WebUI.DAL.Concrete
{
    public class EFDbContextInit : DropCreateDatabaseIfModelChanges<EFDbContext>
    {
        protected override void Seed(EFDbContext context)
        {
            SeedRoles(context);
            SeedTest(context);
            base.Seed(context);
        }

        private void SeedRoles(EFDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            var roles = new[] { Roles.Admin, Roles.Customer, Roles.Manager, Roles.ParkMan, Roles.Driver };

            foreach (var role in roles)
            {
                if (!roleManager.RoleExists(role))
                    roleManager.Create(new IdentityRole(role));
            }
        }

        private void SeedTest(EFDbContext context)
        {
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));

            var user1 = new ApplicationUser { UserName = "JJ5", FirstName = "Janet", LastName = "Jackson" };
            var user2 = new ApplicationUser { UserName = "JJ6", FirstName = "July", LastName = "Jeen" };

            userManager.Create(new ApplicationUser { UserName = "JJ1", FirstName = "Jack1", LastName = "Jackson1" }, "123456");
            userManager.Create(new ApplicationUser { UserName = "JJ2", FirstName = "Jack2", LastName = "Jackson2" }, "123456");
            userManager.Create(new ApplicationUser { UserName = "JJ3", FirstName = "Jack3", LastName = "Jackson3" }, "123456");
            userManager.Create(new ApplicationUser { UserName = "JJ4", FirstName = "Jack4", LastName = "Jackson4" }, "123456");
            userManager.Create(user1, "123456");
            userManager.Create(user2, "123456");
            user1.PreferredRequestDates = new List<TimeOfDay>();
            user1.PreferredRequestDates.Add(new TimeOfDay { Day = DayOfWeek.Sunday, Time = TimeSpan.FromHours(17) });

            var driver1 = new Driver { UserName = "DD5", FirstName = "Vito", LastName = "Mata" };
            var driver2 = new Driver { UserName = "DD6", FirstName = "Bud", LastName = "Malin" };


            userManager.Create(new Driver { UserName = "DD7", FirstName = "Randy", LastName = "Lacomb" });
            userManager.Create(new Driver { UserName = "DD8", FirstName = "Stewart", LastName = "Villegas" });
            userManager.Create(new Driver { UserName = "DD9", FirstName = "Micheal", LastName = "Pascoe" });
            userManager.Create(driver1, "123456");
            userManager.Create(driver2, "123456");

            context.ParkingSlots.Add(new ParkingSlot { Number = "1A", Status = ParkingSlotStatus.Available });
            context.ParkingSlots.Add(new ParkingSlot { Number = "2A", Status = ParkingSlotStatus.Available });
            context.ParkingSlots.Add(new ParkingSlot { Number = "3A", Status = ParkingSlotStatus.Available });
            context.ParkingSlots.Add(new ParkingSlot { Number = "4A", Status = ParkingSlotStatus.Available });
            context.ParkingSlots.Add(new ParkingSlot { Number = "5A", Status = ParkingSlotStatus.Available });

            context.ParkingSlots.Add(new ParkingSlot { Number = "1B", Status = ParkingSlotStatus.Available });
            context.ParkingSlots.Add(new ParkingSlot { Number = "2B", Status = ParkingSlotStatus.Available });
            context.ParkingSlots.Add(new ParkingSlot { Number = "3B", Status = ParkingSlotStatus.Available });
            context.ParkingSlots.Add(new ParkingSlot { Number = "4B", Status = ParkingSlotStatus.Available });
            context.ParkingSlots.Add(new ParkingSlot { Number = "5B", Status = ParkingSlotStatus.Available });

            context.Vans.Add(new Van { Name = "Van1", Latitude = 37.399340f, Longitude = -122.135820f, Color = Color.White.ToArgb() });
            context.Vans.Add(new Van { Name = "Van2", Latitude = 37.398822f, Longitude = -122.134514f, Color = Color.Green.ToArgb() });
            context.SaveChanges();
        }
    }
}