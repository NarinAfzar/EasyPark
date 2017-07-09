using System.Collections.Generic;

namespace EasyPark.WebUI.Areas.CarDriver.ViewModels
{
    public class TaskListViewModel
    {
        public TaskViewModel Current { get; set; }
        public TaskViewModel Finished { get; set; }
        public IEnumerable<TaskViewModel> Assigned { get; set; }
    }
}