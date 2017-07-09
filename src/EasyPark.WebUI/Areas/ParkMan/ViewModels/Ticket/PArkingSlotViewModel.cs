using EasyPark.WebUI.DAL.Entities;

namespace EasyPark.WebUI.Areas.ParkMan.ViewModels.Ticket
{
    public class ParkingSlotViewModel
    {
        public long ID { get; set; }
        public string Number { get; set; }
        public ParkingSlotStatus Status { get; set; }
    }
}