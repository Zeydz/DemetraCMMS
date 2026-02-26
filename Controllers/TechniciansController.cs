using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using dotnet_projektuppgift.Data;
using dotnet_projektuppgift.Models.Entities;
using Microsoft.AspNetCore.Authorization;

namespace dotnet_projektuppgift.Controllers
{
    [Authorize]
    public class TechniciansController : Controller
    {
        /*Database context for accessing technician and related data*/
        private readonly ApplicationDbContext _context;

        public TechniciansController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Technicians
        public async Task<IActionResult> Index()
        {
            /* Load technicians with user accounts*/
            var technicians = await _context.Technicians
                .Include(t => t.User)
                .ToListAsync();
            return View(technicians);
        }

        // GET: Technicians/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
                
                /*Load technician with related user and skills for details view*/
            var technician = await _context.Technicians
                .Include(t => t.User)
                .Include(t => t.TechnicianSkills)
                    .ThenInclude(ts => ts.Skill)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (technician == null)
            {
                return NotFound();
            }

            return View(technician);
        }

        // GET: Technicians/Create
        public IActionResult Create()
        {
            var availableUsers = _context.Users
                .Where(u => u.Technician == null)
                .Select(u => new
                {
                    u.Id,
                    DisplayName = u.FullName + " (" + u.Email + ")"
                })
                .ToList();

            if (!availableUsers.Any())
            {
                TempData["Error"] =
                    "No available users to create technician from. All users already have technician records.";
            }
            
            
            ViewData["UserId"] = new SelectList(availableUsers, "Id", "DisplayName");
            return View();
        }

        // POST: Technicians/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,HireDate,IsActive")] Technician technician)
        {
            if (ModelState.IsValid)
            {
                technician.CreatedAt = DateTime.UtcNow;

                _context.Add(technician);
                await _context.SaveChangesAsync();
                
                var user = await _context.Users.FindAsync(technician.UserId);

                TempData["Success"] =
                    $"Technician '{user?.FullName}' created successfully.";
                
                return RedirectToAction(nameof(Index));
            }
            var availableUsers = _context.Users
                .Where(u => u.Technician == null)
                .Select(u => new { u.Id, DisplayName = u.FullName + " (" + u.Email + ")" })
                .ToList();

            ViewData["UserId"] = new SelectList(availableUsers, "Id", "DisplayName", technician.UserId);
            return View(technician);
        }

        // GET: Technicians/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var technician = await _context.Technicians
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (technician == null)
            {
                return NotFound();
            }
            ViewData["UserName"] = technician.User?.FullName ?? "Unknown";
            return View(technician);
        }

        // POST: Technicians/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,HireDate,CreatedAt,IsActive")] Technician technician)
        {
            if (id != technician.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(technician);
                    await _context.SaveChangesAsync();

                    var user =
                        await _context.Users.FindAsync(technician
                            .UserId);

                    TempData["Success"] =
                        $"Technician '{user?.FullName}' updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TechnicianExists(technician.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            
            /* Reload user info */
            var tech = await _context.Technicians
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", technician.UserId);
            return View(technician);
        }

        // POST: Technicians/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            /*Load technician with related data to check relations*/
            var technician = await _context.Technicians
                .Include(t => t.User)  
                .Include(t => t.AssignedTickets)  
                .Include(t => t.TechnicianSkills)  
                .FirstOrDefaultAsync(t => t.Id == id);
            
            if (technician == null)
            {
                return NotFound();
            }


            /*SAFETY CHECK: Prevent deletion if technician has assigned tickets*/

            if (technician.AssignedTickets.Any())
            {
                TempData["Error"] = $"Cannot delete technician '{technician.Name}' - they have {technician.AssignedTickets.Count} assigned ticket(s).";
                return RedirectToAction(nameof(Index));
            }
            
            /*Safe to delete*/
            _context.Technicians.Remove(technician);
            await _context.SaveChangesAsync();
            
            TempData["Success"] = $"Technician '{technician.Name}' deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool TechnicianExists(int id)
        {
            return _context.Technicians.Any(e => e.Id == id);
        }
    }
}
