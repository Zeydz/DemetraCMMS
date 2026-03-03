using System.ComponentModel.DataAnnotations;
using dotnet_projektuppgift.Models.Enums;

namespace dotnet_projektuppgift.ViewModels;

public class PublicTicketSubmissionViewModel
{
    [Required]
    [StringLength(100)]
    [Display(Name = "Full Name")]
    public string ContactName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string ContactEmail { get; set; } = string.Empty;

    [Phone]
    [StringLength(20)]
    [Display(Name = "Phone Number")]
    public string? ContactPhone { get; set; }

    [Required]
    [Display(Name = "Equipment")]
    public int EquipmentId { get; set; }

    [Required]
    [StringLength(200)]
    [Display(Name = "Issue Summary")]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    [Display(Name = "Description")]
    public string Description { get; set; } = string.Empty;

    [Required]
    public TicketPriority Priority { get; set; }
}