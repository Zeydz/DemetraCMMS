using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dotnet_projektuppgift.Data;
using dotnet_projektuppgift.Models.Entities;
using Microsoft.AspNetCore.Authorization;

namespace dotnet_projektuppgift.Controllers
{
    [Authorize]
    public class SkillsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SkillsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Skills
        public async Task<IActionResult> Index()
        {
            var skills = await _context.Skills
                .Include(s => s.TechnicianSkills)
                .ToListAsync();
            
            return View(skills);
        }

        // GET: Skills/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            /*Load skill with related technicians for details view*/
            var skill = await _context.Skills
                .Include(s => s.TechnicianSkills)
                    .ThenInclude(ts => ts.Technician)
                .ThenInclude(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (skill == null)
            {
                return NotFound();
            }

            return View(skill);
        }

        // GET: Skills/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Skills/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description")] Skill skill)
        {
            if (ModelState.IsValid)
            {
                _context.Add(skill);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = $"Skill '{skill.Name}' created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(skill);
        }

        // GET: Skills/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var skill = await _context.Skills
                .Include(s => s.TechnicianSkills)
                .FirstOrDefaultAsync(s => s.Id == id);
            
            if (skill == null)
            {
                return NotFound();
            }
            
            return View(skill);
        }

        // POST: Skills/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Skill skill)
        {
            if (id != skill.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(skill);
                    await _context.SaveChangesAsync();
                    
                    TempData["Success"] = $"Skill '{skill.Name}' updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SkillExists(skill.Id))
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
            return View(skill);
        }

        // POST: Skills/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            /*Load skill with related technicians to check for dependencies*/
            var skill = await _context.Skills
                .Include(s => s.TechnicianSkills)
                .FirstOrDefaultAsync(s => s.Id == id);
            
            if (skill == null)
            {
                return NotFound();
            }

            /*Check if skill has technicians*/
            if (skill.TechnicianSkills.Any())
            {
                TempData["Error"] = $"Cannot delete skill '{skill.Name}' - it is assigned to {skill.TechnicianSkills.Count} technician(s).";
                return RedirectToAction(nameof(Index));
            }

            _context.Skills.Remove(skill);
            await _context.SaveChangesAsync();
            
            TempData["Success"] = $"Skill '{skill.Name}' deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool SkillExists(int id)
        {
            return _context.Skills.Any(e => e.Id == id);
        }
    }
}
