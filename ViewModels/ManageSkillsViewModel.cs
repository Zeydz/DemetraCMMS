namespace dotnet_projektuppgift.ViewModels;


/*ViewModel for managing a technician's skills*/
public class ManageSkillsViewModel
{
    public int TechnicianId { get; set; }
    public string TechnicianName { get; set; } = string.Empty;
    public List<SkillCheckboxViewModel> AvailableSkills { get; set; } = new();
}


/*ViewModel for displaying a skill as a checkbox*/
public class SkillCheckboxViewModel
{
    public int SkillId { get; set; }
    public string SkillName { get; set; } = string.Empty;
    public string SkillDescription { get; set; } = string.Empty;
    public bool IsSelected { get; set; }
}