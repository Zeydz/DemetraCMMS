namespace dotnet_projektuppgift.Models.Enums;

public enum TicketPriority
{
    /* Low priority - ? days to resolve */
    Low = 0,
    
    /* Medium priority - ? days to resolve */
    Medium = 1,
    
    /* High priority - ? days to resolve */
    High = 2,
    
    /* Critical priority - ? hours to resolve*/
    Critical = 3,
}