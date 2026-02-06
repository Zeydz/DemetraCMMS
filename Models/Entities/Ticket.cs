using System.ComponentModel.DataAnnotations;
using dotnet_projektuppgift.Models.Enums;

namespace dotnet_projektuppgift.Models.Entities;

public class Ticket
{
    public int Id { get; set; }

    [Required] [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    public TicketPriority Priority { get; set; } = TicketPriority.Medium;
    
    [Required]
    public TicketStatus Status { get; set; } = TicketStatus.New;
    
    [Required]
    public int EquipmentId { get; set; }
    
    public string? ReportedById { get; set; }
    
    public int? AssignedToId { get; set; }
    
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    
    public DateTime DueDate { get; set; }
    
    public DateTime? ResolvedDate { get; set; }
    
    public DateTime? ClosedDate { get; set; }
    
    public string? UpdatedById { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    
    [MaxLength(100)]
    [EmailAddress]
    public string? ContactEmail { get; set; }
    
    [MaxLength(100)]
    public string? ContactName { get; set; }
    
    [MaxLength(20)]
    public string? ContactPhone { get; set; }

    /* Navigation properties*/
    
    public Equipment Equipment { get; set; } = null!;
    
    public ApplicationUser? ReportedBy { get; set; }
    
    public Technician? AssignedTo { get; set; }
    
    public ApplicationUser? UpdatedBy { get; set; }
    
    public ICollection<TicketComment> Comments { get; set; } = new List<TicketComment>();
}
