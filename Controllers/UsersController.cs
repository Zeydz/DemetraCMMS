using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dotnet_projektuppgift.Data;
using dotnet_projektuppgift.Models;
using dotnet_projektuppgift.Models.Entities;
using dotnet_projektuppgift.Models.ViewModels;
using dotnet_projektuppgift.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace dotnet_projektuppgift.Controllers
{

    /* Controller to manage user-accounts. Only Admin can access*/
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }
        
        // GET: Users/Index
        public async Task<IActionResult> Index()
        {
            /*Get all users*/
            var users = await _userManager.Users.ToListAsync();
            
            /*Create a list to hold user data with roles*/
            var userViewModels = new List<UserIndexViewModel>();
            
            foreach (var user in users)
            {
                /*Get roles for each user*/
                var roles = await _userManager.GetRolesAsync(user);
                
                /*Check if user has a technician record*/
                var hasTechnicianRecord = await _context.Technicians
                    .AnyAsync(t => t.UserId == user.Id);
                
                userViewModels.Add(new UserIndexViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email ?? "",
                    PhoneNumber = user.PhoneNumber ?? "",
                    EmailConfirmed = user.EmailConfirmed,
                    Roles = roles.ToList(),
                    HasTechnicianRecord = hasTechnicianRecord
                });
            }
            
            return View(userViewModels);
        }


        // GET: Users/Details/5
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            /*Get user roles*/
            var roles = await _userManager.GetRolesAsync(user);
            
            /*Check if user has a technician record*/
            var technician = await _context.Technicians
                .Include(t => t.TechnicianSkills)
                    .ThenInclude(ts => ts.Skill)
                .FirstOrDefaultAsync(t => t.UserId == user.Id);

            var viewModel = new UserDetailsViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? "",
                PhoneNumber = user.PhoneNumber ?? "",
                EmailConfirmed = user.EmailConfirmed,
                Roles = roles.ToList(),
                Technician = technician
            };

            return View(viewModel);
        }
        
        // GET: Users/Create
        public async Task<IActionResult> Create()
        {
            /*Get all available roles*/
            var roles = await _roleManager.Roles.ToListAsync();
            /* First "Name" is for option value, second is for Display*/
            ViewData["Roles"] = new SelectList(roles, "Name", "Name");
            
            return View();
        }


        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                /* Create the user account */
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNumber,
                    EmailConfirmed = true 
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    /*Assign the selected role*/
                    if (!string.IsNullOrEmpty(model.Role))
                    {
                        await _userManager.AddToRoleAsync(user, model.Role);
                    }

                    /* Auto-creeate technician if technician is chosen as role
                     otherwise Admin has to assign technician at technician tab*/

                    if (model.Role == "Technician")
                    {
                        var technician = new Technician
                        {
                            UserId = user.Id,
                            HireDate = model.HireDate ??
                                       DateTime.Today,
                            IsActive = model.IsActive,
                            CreatedAt = DateTime.UtcNow
                        };
                        
                        _context.Technicians.Add(technician);
                        await _context.SaveChangesAsync();
                        
                        TempData["Success"] = $"User '{user.FullName}' and Technician profile created successfully.";
                    }
                    else
                    {
                        TempData["Success"] = $"User '{user.FullName}' created successfully.";
                    }
                    
                    return RedirectToAction(nameof(Index));
                }
            }

             /*If we got here, something probably failed*/
            var roles = await _roleManager.Roles.ToListAsync();
            ViewData["Roles"] = new SelectList(roles, "Name", "Name", model.Role);
            
            return View(model);
        }
        
        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            /*Get current roles for this user*/
            var userRoles = await _userManager.GetRolesAsync(user);
            
            /*Get all available roles*/
            var allRoles = await _roleManager.Roles.ToListAsync();

            var viewModel = new EditUserViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? "",
                PhoneNumber = user.PhoneNumber ?? "",
                CurrentRole = userRoles.FirstOrDefault() ?? ""
            };

            ViewData["Roles"] = new SelectList(allRoles, "Name", "Name");

            
            return View(viewModel);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, EditUserViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                /*Update user properties*/
                user.FullName = model.FullName;
                user.Email = model.Email;
                user.UserName = model.Email; 
                user.PhoneNumber = model.PhoneNumber;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    /* Role change logic with technician record sync*/
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    var currentRole =
                        currentRoles.FirstOrDefault() ?? "";
                    
                    if (!string.IsNullOrEmpty(model.NewRole) && currentRole != model.NewRole)
                    {
                        
                        /*Role is being changed*/
                        var existingTechnician = await _context.Technicians
                            .Include(t => t.AssignedTickets)
                            .Include(t => t.TechnicianSkills)
                            .FirstOrDefaultAsync(t => t.UserId == user.Id);
                        
                        
                        /* CASE 1: Changing FROM "Technician" to something else */
                        if (currentRole == "Technician" &&
                            model.NewRole != "Technician")
                        {
                            if (existingTechnician != null)
                            {
                                /* Cant remove technician if there are assigned tickets */
                                if (existingTechnician.AssignedTickets
                                    .Any())
                                {
                                    TempData["Error"] = $"Cannot change role from Technician - user has {existingTechnician.AssignedTickets.Count} assigned ticket(s). Reassign or close tickets first.";
                                    return RedirectToAction(nameof(Index));
                                }
                                
                                /* Safe to remove technician*/
                                _context.Technicians.Remove(existingTechnician);
                                await _context.SaveChangesAsync();
                                
                                TempData["Success"] = $"User '{user.FullName}' role changed to '{model.NewRole}' and technician profile removed.";
                            }
                        }
                        
                        /* CASE 2: Changing TO "Technician" from another role*/
                        else if (currentRole != "Technician" && model.NewRole == "Technician")
                        {
                            if (existingTechnician == null)
                            {
                                /*Create new technician record with default values*/
                                var technician = new Technician
                                {
                                    UserId = user.Id,
                                    HireDate = DateTime.Today,
                                    IsActive = true,
                                    CreatedAt = DateTime.UtcNow
                                };
                                
                                _context.Technicians.Add(technician);
                                await _context.SaveChangesAsync();
                                
                                TempData["Success"] = $"User '{user.FullName}' role changed to 'Technician' and technician profile created. You can edit hire date in the Technicians section.";
                            }
                        }
                        else
                        {
                            /*Role is changing but not involving Technician role, just update role*/
                            TempData["Success"] = $"User '{user.FullName}' role changed to '{model.NewRole}'.";
                        }
                        
                        /*Remove all current roles*/
                        if (currentRoles.Count > 0)
                        {
                            await _userManager.RemoveFromRolesAsync(user, currentRoles);
                        }
                        
                        /*Add new role*/
                        await _userManager.AddToRoleAsync(user, model.NewRole);
                    }
                    else
                    {
                        TempData["Success"] = $"User '{user.FullName}' updated successfully.";

                    }
                    
                    return RedirectToAction(nameof(Index));
                }
            }
            return RedirectToAction(nameof(Index));
        }
        
        // POST: Users/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            
            /*
            SAFETY CHECK: Prevent deleting yourself
            */
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.Id == user.Id)
            {
                TempData["Error"] = "You cannot delete your own account.";
                return RedirectToAction(nameof(Index));
            }
            
            /*SAFETY CHECK: Check if user has a technician record*/
            var technician = await _context.Technicians
                .Include(t => t.AssignedTickets)
                .FirstOrDefaultAsync(t => t.UserId == user.Id);

            if (technician != null)
            {
                if (technician.AssignedTickets.Any())
                {
                    TempData["Error"] = $"Cannot delete user '{user.FullName}' - they have a technician record with {technician.AssignedTickets.Count} assigned ticket(s). Delete the technician record first.";
                    return RedirectToAction(nameof(Index));
                }
                
                /*Delete technician record first*/
                _context.Technicians.Remove(technician);
                await _context.SaveChangesAsync();
            }

            /*Safe to delete user*/
            var result = await _userManager.DeleteAsync(user);
            
            if (result.Succeeded)
            {
                TempData["Success"] = $"User '{user.FullName}' deleted successfully.";
            }
            else
            {
                TempData["Error"] = "Failed to delete user: " + string.Join(", ", result.Errors.Select(e => e.Description));
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
