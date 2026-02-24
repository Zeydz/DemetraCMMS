using System.ComponentModel.DataAnnotations;

namespace dotnet_projektuppgift.Models.Entities;

public class Technician
{
    public int Id { get; set; }
    
    [Required]
    public string UserId { get; set; } = string.Empty;
    
    [Required]
    public DateTime HireDate { get; set; }
    
    public DateTime CreatedAt { get; set; } =  DateTime.UtcNow;
    
    public bool IsActive { get; set; }
    
    /* Navigation properties*/

    public ApplicationUser User { get; set; } = null!;
    
    public ICollection<TechnicianSkill> TechnicianSkills { get; set; } = new List<TechnicianSkill>();
    public ICollection<Ticket> AssignedTickets { get; set; } = new List<Ticket>();
    
    /* Get from linked User */
    public string Name => User?.FullName ?? "Unknown";
    public string Email => User?.Email ?? "";
    public string PhoneNumber => User?.PhoneNumber ?? "";


}