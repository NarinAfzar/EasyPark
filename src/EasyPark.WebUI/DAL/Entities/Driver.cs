namespace EasyPark.WebUI.DAL.Entities
{
    public class Driver : ApplicationUser
    {
        public DriverStatusType Status { get; set; }

        public long? VanID { get; set; }

        public virtual Van Van { get; set; }
    }
}