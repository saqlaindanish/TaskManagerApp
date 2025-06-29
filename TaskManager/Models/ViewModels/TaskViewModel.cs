namespace TaskManager.Models.ViewModels
{
    public class TaskViewModel
    {
        public string TaskId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string AddedDate { get; set; }
        public string ExpiryDate { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
    }
}
