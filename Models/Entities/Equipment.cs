using System.ComponentModel.DataAnnotations;
using dotnet_projektuppgift.Models.Enums;

namespace dotnet_projektuppgift.Models.Entities;

public class Equipment
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    [MaxLength(50)]
    public string? SerialNumber  { get; set; }

    [Required] 
    public EquipmentStatus Status { get; set; } = EquipmentStatus.Operational;
    
    [Required]
    public int LocationId  { get; set; }
    
    public DateTime? PurchaseDate { get; set; }
    
    public DateTime CreatedAt  { get; set; } = DateTime.UtcNow;

    public Location? Location { get; set; }
    
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    public ICollection<MaintenanceSchedule> MaintenanceSchedules { get; set; } = new List<MaintenanceSchedule>();
}