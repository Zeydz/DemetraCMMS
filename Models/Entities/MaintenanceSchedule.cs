using System.ComponentModel.DataAnnotations;

namespace dotnet_projektuppgift.Models.Entities;

public class MaintenanceSchedule
{
    public int Id { get; set; }
    
    [Required]
    public int EquipmentId { get; set; }
    
    [Required]
    [MaxLength(500)]
    public string TaskDescription { get; set; } = string.Empty;
    
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Interval must be at least 1 day")]
    public int IntervalDays { get; set; }
    
    public DateTime? LastPerformed { get; set; }
    
    [Required]
    public DateTime NextDue { get; set; }

    public bool IsActive { get; set; } = true;

    public Equipment Equipment { get; set; } = null!;
}