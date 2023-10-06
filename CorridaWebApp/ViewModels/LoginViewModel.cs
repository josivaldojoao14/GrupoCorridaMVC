using System.ComponentModel.DataAnnotations;

namespace CorridaWebApp.ViewModels
{
  public class LoginViewModel
  {
    [Display(Name = "Email address")]
    [Required(ErrorMessage = "Email address is required")]
    public string EmailAddress { get; set; }

    [Required(ErrorMessage = "The password is required")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
  }
}
