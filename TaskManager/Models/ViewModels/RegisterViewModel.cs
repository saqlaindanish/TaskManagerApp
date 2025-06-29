using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage ="This field is required.")]
        public string FullName { get; set; }
        
        [Required(ErrorMessage = "This field is required.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}
