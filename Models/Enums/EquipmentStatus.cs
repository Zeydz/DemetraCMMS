namespace dotnet_projektuppgift.Models.Enums;

public enum EquipmentStatus
{
    /* Equipment is fully operational and available for use*/
    Operational = 0,
    
    /* Equipment is currently under maintenance or repair*/
    UnderMaintenance = 1,
    
    /* Equipment is out of service and cannot be used*/
    OutOfService = 2,
}