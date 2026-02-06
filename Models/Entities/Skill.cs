using System.ComponentModel.DataAnnotations;

namespace dotnet_projektuppgift.Models.Entities;

public class Skill
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<TechnicianSkill> TechnicianSkills { get; set; } = new List<TechnicianSkill>();
}