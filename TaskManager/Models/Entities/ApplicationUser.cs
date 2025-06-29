using Microsoft.AspNetCore.Identity;

namespace TaskManager.Models.Entities
{
    public class ApplicationUser: IdentityUser
    {
        public string FullName { get; set; } = string.Empty;

        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>(); // Navigation property for tasks

        public ICollection<Notification> Notifications { get; set; } = new List<Notification>(); // Navigation property for notifications
    }
}
