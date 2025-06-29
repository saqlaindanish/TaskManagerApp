using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models.ViewModels
{

    public enum TaskPriority
    {
        Low, Medium, High
    }
    public class AddTaskViewModel
    {
        public string ? TaskId { get; set; }
        public string? ReturnUrl { get; set; }

        [Required(ErrorMessage = "Task Title is required")]
        public string Title { get; set; }
        [Required(ErrorMessage ="Description is required")]
        public string Description { get; set; }
        [Required(ErrorMessage ="Please set task priority")]
        public TaskPriority Priority { get; set; }

        [Required(ErrorMessage = "Please set task dua date")]
        public DateTime DueDate { get; set; }
    }
}
