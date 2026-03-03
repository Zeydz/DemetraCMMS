using System.ComponentModel.DataAnnotations;
using dotnet_projektuppgift.Models.Enums;

namespace dotnet_projektuppgift.ViewModels;

public class CreateTicketViewModel
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public TicketPriority Priority { get; set; }

    [Required]
    [Display(Name = "Equipment")]
    public int EquipmentId { get; set; }

    [Display(Name = "Assign To (Optional)")]
    public int? AssignedToId { get; set; }
}

public class EditTicketViewModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public TicketPriority Priority { get; set; }

    [Required]
    public TicketStatus Status { get; set; }

    [Required]
    [Display(Name = "Equipment")]
    public int EquipmentId { get; set; }

    [Display(Name = "Assigned To")]
    public int? AssignedToId { get; set; }
}