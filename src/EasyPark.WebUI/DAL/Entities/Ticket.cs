using System.Collections.Generic;

namespace EasyPark.WebUI.DAL.Entities
{
    public class Ticket : IEntity
    {
        public long ID { get; set; }

        public string TicketNumber { get; set; }

        public string Description { get; set; }

        public string LicensePlate { get; set; }

        public string DepartureTime { get; set; }

        public string Color { get; set; }

        public string CustomerID { get; set; }

        public virtual ApplicationUser Customer { get; set; }

        public virtual ICollection<Image> Images { get; set; }

        public virtual ICollection<TicketStatus> TicketStatuses { get; set; }

        public virtual ICollection<Request> Requests { get; set; }
    }
}