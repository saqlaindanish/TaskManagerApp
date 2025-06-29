using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Models.Entities
{
    public class TaskItem
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty; 
        public string Status { get; set; } = "Pending"; // Default status

        public string Priority { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime DueDate { get; set; }


        // relationship
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }       // navigation property

        public ICollection<Notification> Notifications { get; set; } = new List<Notification>(); // navigation property

    }
}
