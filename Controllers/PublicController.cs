using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dotnet_projektuppgift.Data;
using dotnet_projektuppgift.Models.Entities;
using dotnet_projektuppgift.Models.Enums;
using dotnet_projektuppgift.Models.ViewModels;
using dotnet_projektuppgift.ViewModels;

namespace dotnet_projektuppgift.Controllers
{
    public class PublicController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PublicController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        // GET: / (Homepage)
        public async Task<IActionResult> Index()
        {
            /*Load equipment for the dropdown*/
            var equipment = await _context.Equipment
                .Include(e => e.Location)
                .Where(e => e.Status == EquipmentStatus.Operational ||
                            e.Status == EquipmentStatus.UnderMaintenance)
                .OrderBy(e => e.Name)
                .ToListAsync();

            ViewData["Equipment"] = equipment;

            return View();
        }

        // GET: /Public/CheckStatus?ticketId=123
        public async Task<IActionResult> CheckStatus(int? ticketId)
        {
            /*If no ticket ID provided, show search form*/
            if (!ticketId.HasValue)
            {
                return View();
            }

            /* Look up the ticket with information*/
            var ticket = await _context.Tickets
                .Include(t => t.Equipment)
                .ThenInclude(e => e.Location)
                .Include(t => t.AssignedTo)
                .ThenInclude(a => a.User)
                .FirstOrDefaultAsync(t => t.Id == ticketId.Value);

            /*If ticket not found, show error*/
            if (ticket == null)
            {
                TempData["Error"] = $"Ticket #{ticketId} not found. Please check your ticket number and try again.";
                return View();
            }

            /*Show ticket details*/
            return View(ticket);
        }

        // POST: /Public/SubmitTicket
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitTicket(PublicTicketSubmissionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please fill in all required fields correctly.";
                return RedirectToAction("Index", "Public");
            }

            /*Create the ticket*/
            var ticket = new Ticket
            {
                Title = model.Title,
                Description = model.Description,
                Priority = model.Priority,
                Status = TicketStatus.New,
                EquipmentId = model.EquipmentId,
                ReportedById = null, /* No user ID since it's a public submission*/
                ContactName = model.ContactName,
                ContactEmail = model.ContactEmail,
                ContactPhone = model.ContactPhone,
                CreatedDate = DateTime.UtcNow,
                DueDate = CalculateDueDate(model.Priority)
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            /*Redirect to thank you page with ticket number*/
            return RedirectToAction("ThankYou", new { ticketId = ticket.Id });
        }

        // GET: /Public/ThankYou
        public async Task<IActionResult> ThankYou(int ticketId)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Equipment)
                    .ThenInclude(e => e.Location)
                .FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null)
            {
                return RedirectToAction("Index", "Public");
            }

            return View(ticket);
        }

        /*Helper: Calculate due date based on priority*/
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
