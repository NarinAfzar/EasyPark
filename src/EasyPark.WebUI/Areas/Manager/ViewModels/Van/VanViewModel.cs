using System.Collections.Generic;

namespace EasyPark.WebUI.Areas.Manager.ViewModels.Van
{
    public class VanViewModel
    {
        public long ID { get; set; }

        public string Name { get; set; }

        public float Latitude { get; set; }

        public float Longitude { get; set; }

        public IEnumerable<string> Drivers { get; set; }
    }
}