using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Models.Entities
{
    public class Notification
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Message { get; set; }
        public bool IsSeen { get; set; } = false;
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; }

        public string Type {  get; set; }

        // relationships

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        public string ? TaskId { get; set; }
        [ForeignKey("TaskId")]
        public TaskItem ? Task { get; set; }

    }
}
