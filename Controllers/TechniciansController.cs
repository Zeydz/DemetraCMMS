using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using dotnet_projektuppgift.Data;
using dotnet_projektuppgift.Models.Entities;
using dotnet_projektuppgift.Models.ViewModels;
using dotnet_projektuppgift.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace dotnet_projektuppgift.Controllers
{
    [Authorize(Roles = "Admin, Manager")]
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
                .Include(t => t.AssignedTickets)
                .ThenInclude(ticket => ticket.Equipment)
                .ThenInclude(e => e.Location)
                
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


            ViewData["UserId"] = new SelectList(availableUsers, "Id",
                "DisplayName");
            return View();
        }

        // POST: Technicians/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("UserId,HireDate,IsActive")] Technician technician)
        {
            if (ModelState.IsValid)
            {
                technician.CreatedAt = DateTime.UtcNow;

                _context.Add(technician);
                await _context.SaveChangesAsync();

                var user =
                    await _context.Users.FindAsync(technician.UserId);

                TempData["Success"] =
                    $"Technician '{user?.FullName}' created successfully.";

                return RedirectToAction(nameof(Index));
            }

            var availableUsers = _context.Users
                .Where(u => u.Technician == null)
                .Select(u => new
                {
                    u.Id,
                    DisplayName = u.FullName + " (" + u.Email + ")"
                })
                .ToList();

            ViewData["UserId"] = new SelectList(availableUsers, "Id",
                "DisplayName", technician.UserId);
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

            ViewData["UserName"] =
                technician.User?.FullName ?? "Unknown";
            return View(technician);
        }

        // POST: Technicians/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,UserId,HireDate,CreatedAt,IsActive")]
            Technician technician)
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

            ViewData["UserId"] = new SelectList(_context.Users, "Id",
                "Id", technician.UserId);
            return View(technician);
        }

        // POST: Technicians/Delete/5
        [HttpPost]
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
                TempData["Error"] =
                    $"Cannot delete technician '{technician.Name}' - they have {technician.AssignedTickets.Count} assigned ticket(s).";
                return RedirectToAction(nameof(Index));
            }

            /*Safe to delete*/
            _context.Technicians.Remove(technician);
            await _context.SaveChangesAsync();

            TempData["Success"] =
                $"Technician '{technician.Name}' deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        /*Checks if a technician exists by ID*/
        private bool TechnicianExists(int id)
        {
            return _context.Technicians.Any(e => e.Id == id);
        }


        // GET: Technicians/ManageSkills/5
        public async Task<IActionResult> ManageSkills(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var technician = await _context.Technicians
                .Include(t => t.User)
                .Include(t => t.TechnicianSkills)
                .ThenInclude(ts => ts.Skill)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (technician == null)
            {
                return NotFound();
            }

            /*Get all available skills*/
            var allSkills = await _context.Skills.OrderBy(s => s.Name)
                .ToListAsync();

            /*Get IDs of skills this technician already has*/
            var technicianSkillIds = technician.TechnicianSkills
                .Select(ts => ts.SkillId).ToList();

            /*Create ViewModel*/
            var viewModel = new ManageSkillsViewModel
            {
                TechnicianId = technician.Id,
                TechnicianName = technician.Name,
                AvailableSkills = allSkills.Select(skill =>
                    new SkillCheckboxViewModel
                    {
                        SkillId = skill.Id,
                        SkillName = skill.Name,
                        SkillDescription = skill.Description,
                        IsSelected = technicianSkillIds.Contains(skill.Id)
                    }).ToList()
            };

            return View(viewModel);
        }


        // POST: Technicians/ManageSkills/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageSkills(int id,
            List<int> selectedSkills)
        {
            var technician = await _context.Technicians
                .Include(t => t.User)
                .Include(t => t.TechnicianSkills)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (technician == null)
            {
                return NotFound();
            }

            /*selectedSkills will be null if no checkboxes are checked*/
            selectedSkills ??= new List<int>();

            /*Get current skill IDs*/
            var currentSkillIds = technician.TechnicianSkills
                .Select(ts => ts.SkillId).ToList();
            
            /*Find skills to ADD */
            var skillsToAdd = selectedSkills.Except(currentSkillIds)
                .ToList();

            /*Find skills to REMOVE (currently assigned but not selected)*/
            var skillsToRemove = currentSkillIds
                .Except(selectedSkills).ToList();

            /*Remove unchecked skills*/
            if (skillsToRemove.Any())
            {
                var recordsToRemove = technician.TechnicianSkills
                    .Where(ts => skillsToRemove.Contains(ts.SkillId))
                    .ToList();

                _context.TechnicianSkills
                    .RemoveRange(recordsToRemove);
            }

            /*Add newly checked skills*/
            if (skillsToAdd.Any())
            {
                var recordsToAdd = skillsToAdd.Select(skillId =>
                    new TechnicianSkill
                    {
                        TechnicianId = technician.Id,
                        SkillId = skillId
                    }).ToList();

                await _context.TechnicianSkills.AddRangeAsync(
                    recordsToAdd);
            }

            await _context.SaveChangesAsync();

            TempData["Success"] =
                $"Skills updated for {technician.Name}. Added: {skillsToAdd.Count}, Removed: {skillsToRemove.Count}";

            return RedirectToAction(nameof(Details),
                new { id = technician.Id });
        }
    }
}