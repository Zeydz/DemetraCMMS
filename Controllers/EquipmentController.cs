using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using dotnet_projektuppgift.Data;
using dotnet_projektuppgift.Models.Entities;
using Microsoft.AspNetCore.Authorization;

namespace dotnet_projektuppgift.Controllers
{
    [Authorize]
    public class EquipmentController : Controller
    {
        /*Database context for accessing equipment and related data*/
        private readonly ApplicationDbContext _context;

        public EquipmentController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: Equipment/Index
        public async Task<IActionResult> Index()
        {
             /*Load all equipment with their related Location data*/
            var equipment = await _context.Equipment
                .Include(e => e.Location) 
                .ToListAsync();
            
            return View(equipment);
        }


        // GET: Equipment/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            /*Check database for the equipment with related Location*/
            var equipment = await _context.Equipment
                .Include(e => e.Location) 
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (equipment == null)
            {
                return NotFound();
            }

            return View(equipment);
        }
        
        // GET: Equipment/Create
        public IActionResult Create()
        {
            /*Fill the Location dropdown
            This will be available in the view as ViewBag.LocationId*/
            ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "Name");
            
            return View();
        } 
        
        // POST: Equipment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,SerialNumber,Status,LocationId,PurchaseDate")] Equipment equipment)
        {
            if (ModelState.IsValid)
            {
                equipment.CreatedAt = DateTime.UtcNow;
                
                _context.Add(equipment);
                
                await _context.SaveChangesAsync();
                
                /*Set success message that will be displayed on Index page*/
                TempData["Success"] = $"Equipment '{equipment.Name}' created successfully.";
                
                return RedirectToAction(nameof(Index));
            }
            
            /*If validation failed, fill the dropdown and show form again with errors*/
            ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "Name", equipment.LocationId);
            return View(equipment);
        }
        
        // GET: Equipment/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var equipment = await _context.Equipment.FindAsync(id);
            
            if (equipment == null)
            {
                return NotFound();
            }
            
            /*Fill Location dropdown with current location pre-selected*/
            ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "Name", equipment.LocationId);
            
            return View(equipment);
        }
        
        // POST: Equipment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,SerialNumber,Status,LocationId,PurchaseDate,CreatedAt")] Equipment equipment)
        {
            if (id != equipment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(equipment);
                    
                    await _context.SaveChangesAsync();
                    
                    TempData["Success"] = $"Equipment '{equipment.Name}' updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    /*This exception occurs if the equipment was deleted by another user*/
                    if (!EquipmentExists(equipment.Id))
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
            
            ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "Name", equipment.LocationId);
            return View(equipment);
        }
        
        // POST: Equipment/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            /*Load equipment with related tickets and schedules to check dependencies*/
            var equipment = await _context.Equipment
                .Include(e => e.Tickets)                    
                .Include(e => e.MaintenanceSchedules)       
                .FirstOrDefaultAsync(e => e.Id == id);
            
            if (equipment == null)
            {
                return NotFound();
            }
            
            /* SAFETY CHECK: Prevent deletion if equipment has tickets*/
            if (equipment.Tickets.Any())
            {
                TempData["Error"] = $"Cannot delete equipment '{equipment.Name}' - it has {equipment.Tickets.Count} service ticket(s) associated with it.";
                return RedirectToAction(nameof(Index));
            }
            
            /*SAFETY CHECK: Prevent deletion if equipment has maintenance schedules*/
            if (equipment.MaintenanceSchedules.Any())
            {
                TempData["Error"] = $"Cannot delete equipment '{equipment.Name}' - it has {equipment.MaintenanceSchedules.Count} maintenance schedule(s). Remove schedules first.";
                return RedirectToAction(nameof(Index));
            }

            /*Safe to delete, no dependencies*/
            _context.Equipment.Remove(equipment);
            await _context.SaveChangesAsync();
            
            TempData["Success"] = $"Equipment '{equipment.Name}' deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
        
        /*Check if equipment exists in database*/
        private bool EquipmentExists(int id)
        {
            return _context.Equipment.Any(e => e.Id == id);
        }
    }
}