using System.Collections.Generic;

namespace EasyPark.WebUI.DAL.Entities
{
    public class Van : IEntity
    {
        public long ID { get; set; }

        public string Name { get; set; }

        public float Latitude { get; set; }

        public float Longitude { get; set; }

        public int Color { get; set; }

        public virtual ICollection<Driver> Drivers { get; set; }
    }
}