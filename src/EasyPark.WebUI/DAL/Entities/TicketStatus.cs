using EasyPark.WebUI.Models;
using System;

namespace EasyPark.WebUI.DAL.Entities
{
    public class TicketStatus : IEntity
    {
        public long ID { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public long TicketID { get; set; }

        public virtual Ticket Ticket { get; set; }

        public TicketStatusType Type { get; set; }

        public string InChargeId { get; set; }

        public virtual ApplicationUser InCharge { get; set; }

        public long? SlotId { get; set; }

        public virtual ParkingSlot Slot { get; set; }
    }
}