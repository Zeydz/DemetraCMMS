using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using dotnet_projektuppgift.Data;
using dotnet_projektuppgift.Models;
using dotnet_projektuppgift.Models.Entities;
using dotnet_projektuppgift.Models.Enums;
using dotnet_projektuppgift.Models.ViewModels;
using dotnet_projektuppgift.ViewModels;

namespace dotnet_projektuppgift.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TicketsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Tickets
        public async Task<IActionResult> Index()
        {
            /* Load tickets with related data to display*/
            var tickets = await _context.Tickets
                .Include(t => t.Equipment)
                    .ThenInclude(e => e.Location)
                .Include(t => t.ReportedBy)
                .Include(t => t.AssignedTo)
                    .ThenInclude(a => a.User)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();

            return View(tickets);
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.Equipment)
                    .ThenInclude(e => e.Location)
                .Include(t => t.ReportedBy)
                .Include(t => t.AssignedTo)
                    .ThenInclude(a => a.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // GET: Tickets/Create
        public async Task<IActionResult> Create()
        {
            /*Load equipment with location for dropdown. Only Operational or UnderMaintenance*/
            var equipment = await _context.Equipment
                .Include(e => e.Location)
                .Where(e => e.Status == EquipmentStatus.Operational || e.Status == EquipmentStatus.UnderMaintenance)
                .OrderBy(e => e.Name)
                .Select(e => new
                {
                    e.Id,
                    DisplayName = e.Name + " (" + e.Location.Name + ")"
                })
                .ToListAsync();

            /*Load active technicians*/
            var technicians = await _context.Technicians
                .Include(t => t.User)
                .Where(t => t.IsActive)
                .OrderBy(t => t.User.FullName)
                .Select(t => new
                {
                    t.Id,
                    DisplayName = t.User.FullName
                })
                .ToListAsync();

            ViewData["EquipmentId"] = new SelectList(equipment, "Id", "DisplayName");
            ViewData["TechnicianId"] = new SelectList(technicians, "Id", "DisplayName");

            return View();
        }

        // POST: Tickets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTicketViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                
                /*Auto-set status to Assigned if technician is selected, otherwise New*/
                var ticket = new Ticket
                {
                    Title = model.Title,
                    Description = model.Description,
                    Priority = model.Priority,
                    Status = model.AssignedToId.HasValue ? TicketStatus.Assigned : TicketStatus.New,
                    EquipmentId = model.EquipmentId,
                    ReportedById = currentUser.Id,
                    AssignedToId = model.AssignedToId,
                    CreatedDate = DateTime.UtcNow,
                    DueDate = CalculateDueDate(model.Priority)
                };

                _context.Add(ticket);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Ticket '{ticket.Title}' created successfully.";
                return RedirectToAction(nameof(Index));
            }

            /*Reload dropdowns if validation fails*/
            var equipment = await _context.Equipment
                .Include(e => e.Location)
                .Where(e => e.Status == EquipmentStatus.Operational || e.Status == EquipmentStatus.UnderMaintenance)
                .Select(e => new { e.Id, DisplayName = e.Name + " (" + e.Location.Name + ")" })
                .ToListAsync();

            var technicians = await _context.Technicians
                .Include(t => t.User)
                .Where(t => t.IsActive)
                .Select(t => new { t.Id, DisplayName = t.User.FullName })
                .ToListAsync();

            ViewData["EquipmentId"] = new SelectList(equipment, "Id", "DisplayName", model.EquipmentId);
            ViewData["TechnicianId"] = new SelectList(technicians, "Id", "DisplayName", model.AssignedToId);

            return View(model);
        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            /*Load ticket with related data for editing*/
            var ticket = await _context.Tickets
                .Include(t => t.Equipment)
                .Include(t => t.AssignedTo)
                    .ThenInclude(a => a.User)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null)
            {
                return NotFound();
            }

            var viewModel = new EditTicketViewModel
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                Priority = ticket.Priority,
                Status = ticket.Status,
                EquipmentId = ticket.EquipmentId,
                AssignedToId = ticket.AssignedToId
            };

            /*Load equipment*/
            var equipment = await _context.Equipment
                .Include(e => e.Location)
                .Where(e => e.Status == EquipmentStatus.Operational || e.Status == EquipmentStatus.UnderMaintenance)
                .Select(e => new { e.Id, DisplayName = e.Name + " (" + e.Location.Name + ")" })
                .ToListAsync();

            /*Load technicians*/
            var technicians = await _context.Technicians
                .Include(t => t.User)
                .Where(t => t.IsActive)
                .Select(t => new { t.Id, DisplayName = t.User.FullName })
                .ToListAsync();

            ViewData["EquipmentId"] = new SelectList(equipment, "Id", "DisplayName", ticket.EquipmentId);
            ViewData["TechnicianId"] = new SelectList(technicians, "Id", "DisplayName", ticket.AssignedToId);

            return View(viewModel);
        }

        // POST: Tickets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditTicketViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var ticket = await _context.Tickets.FindAsync(id);
                if (ticket == null)
                {
                    return NotFound();
                }

                ticket.Title = model.Title;
                ticket.Description = model.Description;
                ticket.Priority = model.Priority;
                ticket.Status = model.Status;
                ticket.EquipmentId = model.EquipmentId;
                ticket.AssignedToId = model.AssignedToId;

                /*Update due date if priority changed*/
                if (ticket.Priority != model.Priority)
                {
                    ticket.DueDate = CalculateDueDate(model.Priority);
                }

                /*Auto-set status based on assignment*/
                if (model.AssignedToId.HasValue && ticket.Status == TicketStatus.New)
                {
                    ticket.Status = TicketStatus.Assigned;
                }

                _context.Update(ticket);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Ticket '{ticket.Title}' updated successfully.";
                /* Return back to Details tab, assign ticket.Id to id*/
                return RedirectToAction(nameof(Details), new { id = ticket.Id });
            }

            /*Reload dropdowns*/
            var equipment = await _context.Equipment
                .Include(e => e.Location)
                .Select(e => new { e.Id, DisplayName = e.Name + " (" + e.Location.Name + ")" })
                .ToListAsync();

            var technicians = await _context.Technicians
                .Include(t => t.User)
                .Where(t => t.IsActive)
                .Select(t => new { t.Id, DisplayName = t.User.FullName })
                .ToListAsync();

            ViewData["EquipmentId"] = new SelectList(equipment, "Id", "DisplayName", model.EquipmentId);
            ViewData["TechnicianId"] = new SelectList(technicians, "Id", "DisplayName", model.AssignedToId);

            return RedirectToAction(nameof(Index));
        }

        // POST: Tickets/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Equipment)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null)
            {
                return NotFound();
            }

            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Ticket '{ticket.Title}' deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        /*Helper: Calculate due date based on priority. This is where the counting happens*/
        private DateTime CalculateDueDate(TicketPriority priority)
        {
            return priority switch
            {
                TicketPriority.Critical => DateTime.UtcNow.AddHours(4),
                TicketPriority.High => DateTime.UtcNow.AddHours(24),
                TicketPriority.Medium => DateTime.UtcNow.AddDays(3),
                TicketPriority.Low => DateTime.UtcNow.AddDays(7),
                _ => DateTime.UtcNow.AddDays(3)
            };
        }
    }
}