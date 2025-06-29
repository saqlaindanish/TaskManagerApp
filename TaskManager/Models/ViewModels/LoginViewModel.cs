using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "This field is required.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
