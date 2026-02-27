namespace dotnet_projektuppgift.ViewModels;

public class UserIndexViewModel
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool EmailConfirmed { get; set; }
    
        /*List of role names assigned to the user ("Admin", "Technician")*/
    public List<string> Roles { get; set; } = new();
    public bool HasTechnicianRecord { get; set; }
}