using EasyPark.WebUI.DAL.Abstract;
using EasyPark.WebUI.DAL.Entities;
using EasyPark.WebUI.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace EasyPark.WebUI.DAL.Concrete
{
    public class EFDbContext : IdentityDbContext<ApplicationUser>, IDbContext
    {
        public EFDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        static EFDbContext()
        {
            Database.SetInitializer(new EFDbContextInit());
        }

        public void ChangeState<T>(T entity, EntityState state) where T : class, IEntity
        {
            Entry<T>(entity).State = state;
        }

        public IDbSet<Image> Images { get; set; }
        public IDbSet<ParkingSlot> ParkingSlots { get; set; }
        public IDbSet<Ticket> Tickets { get; set; }
        public IDbSet<TicketStatus> TicketStatuses { get; set; }
        public IDbSet<Request> Requests { get; set; }
        public IDbSet<Van> Vans { get; set; }

        public static EFDbContext Create()
        {
            return new EFDbContext();
        }
    }
}