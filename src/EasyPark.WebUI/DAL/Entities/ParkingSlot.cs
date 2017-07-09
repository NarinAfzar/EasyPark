using System.Collections.Generic;

namespace EasyPark.WebUI.DAL.Entities
{
    public class ParkingSlot : IEntity
    {
        public long ID { get; set; }

        public string Number { get; set; }

        public ParkingSlotStatus Status { get; set; }

        public virtual ICollection<TicketStatus> TicketStatuse { get; set; }
    }
}