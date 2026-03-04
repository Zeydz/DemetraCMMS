using dotnet_projektuppgift.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using dotnet_projektuppgift.Models;
using dotnet_projektuppgift.Models.Entities;
using dotnet_projektuppgift.Models.Enums;

namespace dotnet_projektuppgift
{
    /*Seeds the database with initial data for development and testing*/
    /*https://learn.microsoft.com/en-us/ef/core/modeling/data-seeding the tutorial I followed*/
    public static class DbSeeder
    {
        /* Gets called from Program.cs*/
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            /*Ensure database is created*/
            await context.Database.MigrateAsync();

            /*Seed Roles*/
            await SeedRolesAsync(roleManager);

            /*Seed Default Admin User*/
            await SeedAdminUserAsync(userManager);

            /*Seed Locations*/
            await SeedLocationsAsync(context);

            /*Seed Skills*/
            await SeedSkillsAsync(context);

            /*Seed Equipment*/
            await SeedEquipmentAsync(context);

            /*Seed Technicians*/
            await SeedTechniciansAsync(context, userManager);

            /*Seed Sample Tickets*/
            await SeedTicketsAsync(context);

            await context.SaveChangesAsync();
        }
        
        /*Creates the default roles: Admin, Manager, Technician, User*/
        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "Manager", "Technician", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                    Console.WriteLine($"✅ Role created: {role}");
                }
            }
        }
        
        /*Creates a default admin user for immediate login*/
        private static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager)
        {
            var adminEmail = "admin@maintenance.local";
            
            /* Only create User if there is none */
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "System Administrator",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                    Console.WriteLine($"✅ Admin user created: {adminEmail}");
                    Console.WriteLine($"   Password: Admin123!");
                }
            }
        }
        
        /*Creates sample locations (buildings and floors)*/
        private static async Task SeedLocationsAsync(ApplicationDbContext context)
        {
            if (await context.Locations.AnyAsync())
                return;

            var locations = new[]
            {
                new Location
                {
                    Name = "Building A - Floor 1",
                    Description = "Ground floor of Building A",
                    Building = "Building A",
                    Floor = "Floor 1"
                },
                new Location
                {
                    Name = "Building A - Floor 2",
                    Description = "Second floor of Building A",
                    Building = "Building A",
                    Floor = "Floor 2"
                },
                new Location
                {
                    Name = "Building B - Floor 1",
                    Description = "Ground floor of Building B",
                    Building = "Building B",
                    Floor = "Floor 1"
                },
                new Location
                {
                    Name = "Main Campus - Basement",
                    Description = "Basement level with utilities and storage",
                    Building = "Main Campus",
                    Floor = "Basement"
                }
            };

            await context.Locations.AddRangeAsync(locations);
            await context.SaveChangesAsync();
            Console.WriteLine($"✅ {locations.Length} locations created");
        }


        /*Creates sample skills that technicians can have*/
        private static async Task SeedSkillsAsync(ApplicationDbContext context)
        {
            /* If there are skills already, return */
            if (await context.Skills.AnyAsync())
            {
                return;
            }
                

            var skills = new[]
            {
                new Skill
                {
                    Name = "Electrical",
                    Description = "Electrical systems, wiring, fire alarms, lighting"
                },
                new Skill
                {
                    Name = "HVAC",
                    Description = "Heating, ventilation, and air conditioning systems"
                },
                new Skill
                {
                    Name = "Plumbing",
                    Description = "Water systems, pipes, drainage, fixtures"
                },
                new Skill
                {
                    Name = "IT",
                    Description = "Network infrastructure, servers, computers"
                },
                new Skill
                {
                    Name = "Carpentry",
                    Description = "Doors, windows, furniture, woodwork"
                },
                new Skill
                {
                    Name = "General Maintenance",
                    Description = "Basic repairs, cleaning, minor fixes"
                }
            };

            await context.Skills.AddRangeAsync(skills);
            await context.SaveChangesAsync();
            Console.WriteLine($"✅ {skills.Length} skills created");
        }
        
        /*Creates sample equipment at various locations*/
        private static async Task SeedEquipmentAsync(ApplicationDbContext context)
        {
            /* If equipment already exist, return*/
            if (await context.Equipment.AnyAsync())
            {
                return;
            }
              

            /*Get locations for foreign keys*/
            var buildingA1 = await context.Locations.FirstAsync(l => l.Name == "Building A - Floor 1");
            var buildingA2 = await context.Locations.FirstAsync(l => l.Name == "Building A - Floor 2");
            var buildingB1 = await context.Locations.FirstAsync(l => l.Name == "Building B - Floor 1");
            var basement = await context.Locations.FirstAsync(l => l.Name == "Main Campus - Basement");

            var equipment = new[]
            {
                new Equipment
                {
                    Name = "Fire Alarm System - Building A Floor 2",
                    Description = "Addressable fire alarm system with smoke detectors",
                    SerialNumber = "FA-2024-001",
                    Status = EquipmentStatus.Operational,
                    LocationId = buildingA2.Id,
                    PurchaseDate = new DateTime(2023, 1, 15)
                },
                new Equipment
                {
                    Name = "HVAC Unit #1 - Building A Floor 1",
                    Description = "Central air conditioning and heating unit, 5-ton capacity",
                    SerialNumber = "HVAC-2024-101",
                    Status = EquipmentStatus.Operational,
                    LocationId = buildingA1.Id,
                    PurchaseDate = new DateTime(2022, 6, 20)
                },
                new Equipment
                {
                    Name = "HVAC Unit #2 - Building B Floor 1",
                    Description = "Central air conditioning unit, 3-ton capacity",
                    SerialNumber = "HVAC-2024-102",
                    Status = EquipmentStatus.UnderMaintenance,
                    LocationId = buildingB1.Id,
                    PurchaseDate = new DateTime(2022, 6, 20)
                },
                new Equipment
                {
                    Name = "Elevator - Building B",
                    Description = "Passenger elevator, 8-person capacity",
                    SerialNumber = "ELEV-2024-001",
                    Status = EquipmentStatus.Operational,
                    LocationId = buildingB1.Id,
                    PurchaseDate = new DateTime(2020, 3, 10)
                },
                new Equipment
                {
                    Name = "Network Router - Main Switch",
                    Description = "Core network router for campus network",
                    SerialNumber = "NET-2024-001",
                    Status = EquipmentStatus.Operational,
                    LocationId = basement.Id,
                    PurchaseDate = new DateTime(2023, 9, 5)
                },
                new Equipment
                {
                    Name = "Water Heater - Basement",
                    Description = "Industrial water heater, 100-gallon capacity",
                    SerialNumber = "WH-2024-001",
                    Status = EquipmentStatus.Operational,
                    LocationId = basement.Id,
                    PurchaseDate = new DateTime(2021, 11, 18)
                }
            };

            await context.Equipment.AddRangeAsync(equipment);
            await context.SaveChangesAsync();
            Console.WriteLine($"✅ {equipment.Length} equipment items created");
        }


        /*Creates sample technicians with assigned skills*/
        private static async Task SeedTechniciansAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            if (await context.Technicians.AnyAsync())
            {
                return;
            }
                

            /*Get skills for assignment*/
            var electrical = await context.Skills.FirstAsync(s => s.Name == "Electrical");
            var hvac = await context.Skills.FirstAsync(s => s.Name == "HVAC");
            var plumbing = await context.Skills.FirstAsync(s => s.Name == "Plumbing");
            var it = await context.Skills.FirstAsync(s => s.Name == "IT");
            var generalMaint = await context.Skills.FirstAsync(s => s.Name == "General Maintenance");
            
            
            /*John Doe - User account*/
            var johnUser = new ApplicationUser
            {
                UserName = "john.doe@maintenance.local",
                Email = "john.doe@maintenance.local",
                FullName = "John Doe",       
                PhoneNumber = "555-0101",    
                EmailConfirmed = true
            };
            await userManager.CreateAsync(johnUser, "Tech123!");
            await userManager.AddToRoleAsync(johnUser, "Technician");

            /*Jane Smith - User account*/
            var janeUser = new ApplicationUser
            {
                UserName = "jane.smith@maintenance.local",
                Email = "jane.smith@maintenance.local",
                FullName = "Jane Smith",    
                PhoneNumber = "555-0102",  
                EmailConfirmed = true
            };
            await userManager.CreateAsync(janeUser, "Tech123!");
            await userManager.AddToRoleAsync(janeUser, "Technician");

            /*Bob Johnson - User account*/
            var bobUser = new ApplicationUser
            {
                UserName = "bob.johnson@maintenance.local",
                Email = "bob.johnson@maintenance.local",
                FullName = "Bob Johnson",   
                PhoneNumber = "555-0103",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(bobUser, "Tech123!");
            await userManager.AddToRoleAsync(bobUser, "Technician");
            
            /*Create technician records*/
            
            var john = new Technician
            {
                UserId = johnUser.Id,                    
                HireDate = new DateTime(2023, 1, 15),  
                IsActive = true
            };

            var jane = new Technician
            {
                UserId = janeUser.Id,                    
                HireDate = new DateTime(2023, 3, 20),
                IsActive = true
            };

            var bob = new Technician
            {
                UserId = bobUser.Id,                     
                HireDate = new DateTime(2023, 5, 10),
                IsActive = true
            };

            await context.Technicians.AddRangeAsync(john, jane, bob);
            await context.SaveChangesAsync();
            
            /*Assign skills to technicians*/
            
            var techSkills = new[]
            {
                new TechnicianSkill 
                { 
                    TechnicianId = john.Id, 
                    SkillId = electrical.Id
                },
                new TechnicianSkill 
                { 
                    TechnicianId = john.Id, 
                    SkillId = hvac.Id
                },
                
                new TechnicianSkill 
                { 
                    TechnicianId = jane.Id, 
                    SkillId = plumbing.Id
                },
                new TechnicianSkill 
                { 
                    TechnicianId = jane.Id, 
                    SkillId = generalMaint.Id
                },
                
                new TechnicianSkill 
                { 
                    TechnicianId = bob.Id, 
                    SkillId = it.Id
                },
                new TechnicianSkill 
                { 
                    TechnicianId = bob.Id, 
                    SkillId = electrical.Id
                }
            };
            await context.TechnicianSkills.AddRangeAsync(techSkills);
            await context.SaveChangesAsync();
            
            Console.WriteLine($"✅ 3 technicians created with skills");
            Console.WriteLine($" - john.doe@maintenance.local (Password: Tech123!)");
            Console.WriteLine($" - jane.smith@maintenance.local (Password: Tech123!)");
            Console.WriteLine($" - bob.johnson@maintenance.local (Password: Tech123!)");
        }
        
        /*Creates a couple of sample tickets for testing*/
        private static async Task SeedTicketsAsync(ApplicationDbContext context)
        {
            if (await context.Tickets.AnyAsync())
            {
                return; 
            }
            /*Get references*/
            var fireAlarm = await context.Equipment.FirstAsync(e => e.Name.Contains("Fire Alarm"));
            var hvacUnit = await context.Equipment.FirstAsync(e => e.Name.Contains("HVAC Unit #1"));
            var adminUser = await context.Users.FirstAsync(u => u.Email == "admin@maintenance.local");
            
            /*Get Jane's technician record*/
            var janeUser = await context.Users.FirstAsync(u => u.Email == "jane.smith@maintenance.local");
            var jane = await context.Technicians.FirstAsync(t => t.UserId == janeUser.Id);

            var tickets = new[]
            {
                new Ticket
                {
                    Title = "Fire alarm beeping intermittently",
                    Description = "The fire alarm system in Building A Floor 2 has been beeping every few minutes. Need immediate inspection.",
                    Priority = TicketPriority.Critical,
                    Status = TicketStatus.New,
                    EquipmentId = fireAlarm.Id,
                    ReportedById = adminUser.Id,
                    CreatedDate = DateTime.Now.AddHours(-2),
                    DueDate = DateTime.Now.AddHours(2)
                },
                new Ticket
                {
                    Title = "HVAC not cooling properly",
                    Description = "HVAC Unit #1 is running but not cooling the space adequately. Temperature is 78°F instead of target 72°F.",
                    Priority = TicketPriority.Medium,
                    Status = TicketStatus.Assigned,
                    EquipmentId = hvacUnit.Id,
                    ReportedById = adminUser.Id,
                    AssignedToId = jane.Id,
                    CreatedDate = DateTime.Now.AddDays(-1),
                    DueDate = DateTime.Now.AddDays(2)
                }
            };

            await context.Tickets.AddRangeAsync(tickets);
            await context.SaveChangesAsync();
            Console.WriteLine($"✅ {tickets.Length} sample tickets created");
        }
    }
}