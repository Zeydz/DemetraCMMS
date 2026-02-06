using System.ComponentModel.DataAnnotations;

namespace dotnet_projektuppgift.Models.Entities;

public class Technician
{
    public int Id { get; set; }
    
    [Required]
    public string UserId { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; }  = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string PhoneNumber { get; set; }
    
    [Required]
    public DateTime HireDate { get; set; }
    
    public DateTime CreatedAt { get; set; } =  DateTime.UtcNow;
    
    /* Navigation properties*/

    public ApplicationUser User { get; set; } = null!;
    
    public ICollection<TechnicianSkill> TechnicianSkills { get; set; } = new List<TechnicianSkill>();

    public ICollection<Ticket> AssignedTickets { get; set; } = new List<Ticket>();
    
    /* Not stored in the database */
    public string FullName => $"{FirstName} {LastName}";


}