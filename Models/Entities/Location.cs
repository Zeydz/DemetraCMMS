using System.ComponentModel.DataAnnotations;

namespace dotnet_projektuppgift.Models.Entities;

public class Location
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
   [Required]
   [MaxLength(50)]
   public string Building { get; set; }
    
   
  [Required]
  [MaxLength(20)]
   public string Floor { get; set; } 
   
   
   public DateTime CreatedAt { get; set; } =  DateTime.UtcNow;

   public ICollection<Equipment> Equipment { get; set; } = new List<Equipment>();
}