using dotnet_projektuppgift.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace dotnet_projektuppgift.Models;

public class ApplicationUser : IdentityUser
{
    /* Extends IdentityUser with custom properties */
    
    /* If user is a technician, this links to their profile*/
    public string FullName { get; set; } = string.Empty;
    
    public Technician? Technician { get; set; }
    
    public ICollection<Ticket> ReportedTickets { get; set; } = new List<Ticket>();
    
    public ICollection<Ticket> UpdatedTickets { get; set; } = new List<Ticket>();
    
    public ICollection<TicketComment> Comments { get; set; } = new List<TicketComment>();
}