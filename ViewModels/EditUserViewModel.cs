using System.ComponentModel.DataAnnotations;

namespace dotnet_projektuppgift.Models.ViewModels;

public class EditUserViewModel
{
    public string Id { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Phone]
    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }

    public string CurrentRole { get; set; } = string.Empty;
    
    public string? NewRole { get; set; }
}