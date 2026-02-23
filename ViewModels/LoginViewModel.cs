using System.ComponentModel.DataAnnotations;

namespace dotnet_projektuppgift.ViewModels;

/*The data we expect to receive when the login form is submitted.
Property names here must match the name="" attributes in Login.cshtml.*/
public class LoginViewModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }
}