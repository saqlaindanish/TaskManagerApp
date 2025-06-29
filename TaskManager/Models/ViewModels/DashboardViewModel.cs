using TaskManager.Models.Entities;

namespace TaskManager.Models.ViewModels
{
    public class DashboardViewModel
    {

        public List<TaskItem> NewTasks { get; set; }
        public List<TaskItem> PendingTasks { get; set; }
        public List<TaskItem> In_ProgressTasks { get; set; }
        public List<TaskItem> CompetedTasks { get; set; }
        public List<TaskItem> TasksDueToday { get; set; }

    }
}
