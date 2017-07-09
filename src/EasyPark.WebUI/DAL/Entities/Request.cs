using System;

namespace EasyPark.WebUI.DAL.Entities
{
    public class Request : IEntity
    {
        public long ID { get; set; }

        public DateTime RequestDate { get; set; }

        public bool Read { get; set; }

        public long TicketId { get; set; }

        public virtual Ticket Ticket { get; set; }
    }
}