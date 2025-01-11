using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace DASS.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "First Name is required")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Display(Name = "Middle Name")]
        public string? MiddleName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Password is required")]
       [StringLength(40, MinimumLength = 6, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.")]
        [DataType(DataType.Password)]
		[Compare("ConfirmPassword", ErrorMessage = "Password does not match.")]
		public string Password { get; set; }
        

        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
		[Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}
