using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dotnet_projektuppgift.Data;
using dotnet_projektuppgift.Models;
using dotnet_projektuppgift.Models.Enums;
using dotnet_projektuppgift.ViewModels;

namespace dotnet_projektuppgift.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            
            /*Build statistics*/
            var viewModel = new DashboardViewModel
            {
                /*Ticket statistics*/
                TotalTickets = await _context.Tickets.CountAsync(),
                NewTickets = await _context.Tickets.CountAsync(t => t.Status == TicketStatus.New),
                AssignedTickets = await _context.Tickets.CountAsync(t => t.Status == TicketStatus.Assigned),
                InProgressTickets = await _context.Tickets.CountAsync(t => t.Status == TicketStatus.InProgress),
                ResolvedTickets = await _context.Tickets.CountAsync(t => t.Status == TicketStatus.Resolved),
                ClosedTickets = await _context.Tickets.CountAsync(t => t.Status == TicketStatus.Closed),
                OverdueTickets = await _context.Tickets.CountAsync(t => 
                    t.DueDate < DateTime.UtcNow && 
                    t.Status != TicketStatus.Resolved && 
                    t.Status != TicketStatus.Closed),
                
                /*Priority breakdown*/
                CriticalTickets = await _context.Tickets.CountAsync(t => t.Priority == TicketPriority.Critical),
                HighTickets = await _context.Tickets.CountAsync(t => t.Priority == TicketPriority.High),
                MediumTickets = await _context.Tickets.CountAsync(t => t.Priority == TicketPriority.Medium),
                LowTickets = await _context.Tickets.CountAsync(t => t.Priority == TicketPriority.Low),
                
                /*Equipment statistics*/
                TotalEquipment = await _context.Equipment.CountAsync(),
                OperationalEquipment = await _context.Equipment.CountAsync(e => e.Status == EquipmentStatus.Operational),
                EquipmentUnderMaintenance = await _context.Equipment.CountAsync(e => e.Status == EquipmentStatus.UnderMaintenance),
                OutOfServiceEquipment = await _context.Equipment.CountAsync(e => e.Status == EquipmentStatus.OutOfService),
                
                /*Location statistics*/
                TotalLocations = await _context.Locations.CountAsync(),
                
                /*Technician statistics*/
                TotalTechnicians = await _context.Technicians.CountAsync(),
                ActiveTechnicians = await _context.Technicians.CountAsync(t => t.IsActive),
                InactiveTechnicians = await _context.Technicians.CountAsync(t => !t.IsActive),
                
                /*User statistics*/
                TotalUsers = await _userManager.Users.CountAsync(),
                
                /*Skills*/
                TotalSkills = await _context.Skills.CountAsync(),
                
                /*Recent tickets (last 5)*/
                RecentTickets = await _context.Tickets
                    .OrderByDescending(t => t.CreatedDate)
                    .Take(5)
                    .Select(t => new RecentTicket
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Status = t.Status.ToString(),
                        Priority = t.Priority.ToString(),
                        CreatedDate = t.CreatedDate
                    })
                    .ToListAsync()
            };
            
            /* Show current user */
            ViewData["UserName"] = currentUser?.FullName ?? "User";
            
            return View(viewModel);
        }
    }
}
