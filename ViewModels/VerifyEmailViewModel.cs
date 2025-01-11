using System.ComponentModel.DataAnnotations;

namespace DASS.ViewModels
{
    public class VerifyEmailViewModel
    {
        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }


    }
}
