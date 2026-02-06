using dotnet_projektuppgift.Models;
using dotnet_projektuppgift.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace dotnet_projektuppgift.Data;

/* Using Fluent API to get more freedom with configuration and relations. I found the docs here:
https://learn.microsoft.com/en-us/ef/ef6/modeling/code-first/fluent/types-and-properties
*/
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    /*DbSets for all entities (tables)*/
    
    public DbSet<Location> Locations { get; set; }
    public DbSet<Equipment> Equipment { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<Technician> Technicians { get; set; }
    public DbSet<TechnicianSkill> TechnicianSkills { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<TicketComment> TicketComments { get; set; }
    public DbSet<MaintenanceSchedule> MaintenanceSchedules { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        /* Entity configurations */
        
        /*Location configuration*/
        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(location => location.Id);
            entity.Property(location => location.Name).IsRequired().HasMaxLength(100);
            entity.Property(location => location.Building).IsRequired().HasMaxLength(50);
            entity.Property(location => location.Floor).IsRequired().HasMaxLength(20);
            entity.Property(location => location.Description).IsRequired().HasMaxLength(500);

            /* Prevent duplicates of name*/
            entity.HasIndex(location => location.Name).IsUnique();
        });
        
    }
}