using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using dotnet_projektuppgift.Data;
using dotnet_projektuppgift.Models.Entities;

namespace dotnet_projektuppgift.Controllers
{
    public class MaintenanceSchedulesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MaintenanceSchedulesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MaintenanceSchedules
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.MaintenanceSchedules.Include(m => m.Equipment);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: MaintenanceSchedules/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var maintenanceSchedule = await _context.MaintenanceSchedules
                .Include(m => m.Equipment)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (maintenanceSchedule == null)
            {
                return NotFound();
            }

            return View(maintenanceSchedule);
        }

        // GET: MaintenanceSchedules/Create
        public IActionResult Create()
        {
            ViewData["EquipmentId"] = new SelectList(_context.Equipment, "Id", "Name");
            return View();
        }

        // POST: MaintenanceSchedules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EquipmentId,TaskDescription,IntervalDays,LastPerformed,NextDue,IsActive")] MaintenanceSchedule maintenanceSchedule)
        {
            if (ModelState.IsValid)
            {
                _context.Add(maintenanceSchedule);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EquipmentId"] = new SelectList(_context.Equipment, "Id", "Name", maintenanceSchedule.EquipmentId);
            return View(maintenanceSchedule);
        }

        // GET: MaintenanceSchedules/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var maintenanceSchedule = await _context.MaintenanceSchedules.FindAsync(id);
            if (maintenanceSchedule == null)
            {
                return NotFound();
            }
            ViewData["EquipmentId"] = new SelectList(_context.Equipment, "Id", "Name", maintenanceSchedule.EquipmentId);
            return View(maintenanceSchedule);
        }

        // POST: MaintenanceSchedules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EquipmentId,TaskDescription,IntervalDays,LastPerformed,NextDue,IsActive")] MaintenanceSchedule maintenanceSchedule)
        {
            if (id != maintenanceSchedule.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(maintenanceSchedule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MaintenanceScheduleExists(maintenanceSchedule.Id))
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
            ViewData["EquipmentId"] = new SelectList(_context.Equipment, "Id", "Name", maintenanceSchedule.EquipmentId);
            return View(maintenanceSchedule);
        }

        // GET: MaintenanceSchedules/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var maintenanceSchedule = await _context.MaintenanceSchedules
                .Include(m => m.Equipment)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (maintenanceSchedule == null)
            {
                return NotFound();
            }

            return View(maintenanceSchedule);
        }

        // POST: MaintenanceSchedules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var maintenanceSchedule = await _context.MaintenanceSchedules.FindAsync(id);
            if (maintenanceSchedule != null)
            {
                _context.MaintenanceSchedules.Remove(maintenanceSchedule);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MaintenanceScheduleExists(int id)
        {
            return _context.MaintenanceSchedules.Any(e => e.Id == id);
        }
    }
}
