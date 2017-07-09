using System;

namespace EasyPark.WebUI.DAL.Entities
{
    public class TimeOfDay : IEntity
    {
        public long ID { get; set; }

        public DayOfWeek Day { get; set; }

        public TimeSpan Time { get; set; }
    }
}