namespace dotnet_projektuppgift.ViewModels;


/*ViewModel for the admin dashboard showing system statistics for the logged in user*/

public class DashboardViewModel
{
    /*Ticket statistics*/
    public int TotalTickets { get; set; }
    public int NewTickets { get; set; }
    public int AssignedTickets { get; set; }
    public int InProgressTickets { get; set; }
    public int ResolvedTickets { get; set; }
    public int ClosedTickets { get; set; }
    public int OverdueTickets { get; set; }
    
    /*Priority breakdown*/
    public int CriticalTickets { get; set; }
    public int HighTickets { get; set; }
    public int MediumTickets { get; set; }
    public int LowTickets { get; set; }
    
    /*Equipment & Location Statistics*/
    public int TotalEquipment { get; set; }
    public int OperationalEquipment { get; set; }
    public int EquipmentUnderMaintenance { get; set; }
    public int OutOfServiceEquipment { get; set; }
    public int TotalLocations { get; set; }
    
    /*Technician Statistics*/
    public int TotalTechnicians { get; set; }
    public int ActiveTechnicians { get; set; }
    public int InactiveTechnicians { get; set; }
    
    /*User Statistics*/
    public int TotalUsers { get; set; }
    
    /*Skills*/
    public int TotalSkills { get; set; }
    
    /*Recent Activity*/ 
    public List<RecentTicket> RecentTickets { get; set; } = new();
}


/*Simplified ticket info for recent activity*/
public class RecentTicket
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}