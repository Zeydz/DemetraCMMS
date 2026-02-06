using System.ComponentModel.DataAnnotations;

namespace dotnet_projektuppgift.Models.Entities;

public class TicketComment
{
    [Required]
    public string UserId { get; set; } =  string.Empty;
    
    [Required]
    [MaxLength(200)]
    public string CommentText { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /* Navigation properties */
    public Ticket Ticket { get; set; } = null!;

    public ApplicationUser User { get; set; } = null!;
}