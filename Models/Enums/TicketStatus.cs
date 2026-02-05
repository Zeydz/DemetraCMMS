namespace dotnet_projektuppgift.Models.Enums;

public enum TicketStatus
{
    /* Ticket has been created but no techician has been assigned. */
    New = 0,
    
    /* Ticket has been assigned to a technician but work has not yet started*/
    Assigned = 1, 
    
    /* Technician is working on the ticket */
    InProgress = 2,
    
    /* Issue has been resolved, awaiting confirmation*/
    Resolved = 3,
    
    /* Ticket is verified and closed */
    Closed = 4
}