using System.ComponentModel.DataAnnotations;

namespace dotnet_projektuppgift.Models.Entities;

public class TechnicianSkill
{
    [Required] public int TechnicianId { get; set; }

    [Required] public int SkillId { get; set; }

    public DateTime AssignedDate { get; set; } = DateTime.UtcNow;
    
    /* Navigation properties*/

    public Technician Technician { get; set; } = null!;

    public Skill Skill { get; set; } = null!;
}