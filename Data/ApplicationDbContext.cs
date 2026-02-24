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

    /* FLUENT API */
    
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
            entity.Property(location => location.Description).HasMaxLength(500);

            /* Prevent duplicates of name*/
            entity.HasIndex(location => location.Name).IsUnique();
        });

        /* Equipment configuration */
        modelBuilder.Entity<Equipment>(entity =>
        {
            entity.HasKey(equipment => equipment.Id);
            entity.Property(equipment => equipment.Name).IsRequired().HasMaxLength(100);
            entity.Property(equipment => equipment.Description).HasMaxLength(500);
            entity.Property(equipment => equipment.SerialNumber).HasMaxLength(50);
            entity.Property(equipment => equipment.Status).IsRequired();

            entity.HasIndex(equipment => equipment.Name).IsUnique();

            /* Relations */

            entity.HasOne(equipment => equipment.Location)
                .WithMany(location => location.Equipment)
                .HasForeignKey(equipment => equipment.LocationId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        /* Skill configuration */
        modelBuilder.Entity<Skill>(entity =>
        {
            entity.HasKey(skill => skill.Id);
            entity.Property(skill => skill.Name).IsRequired().HasMaxLength(50);
            entity.Property(skill => skill.Description).HasMaxLength(500);

            entity.HasIndex(skill => skill.Name).IsUnique();
        });
        
        /* Technician configuration*/
        modelBuilder.Entity<Technician>(entity =>
        {
            entity.HasKey(technician => technician.Id);
            entity.Property(technician => technician.UserId).IsRequired();
            entity.Property(technician => technician.FirstName).IsRequired().HasMaxLength(50);
            entity.Property(technician => technician.LastName).IsRequired().HasMaxLength(50);
            entity.Property(technician => technician.PhoneNumber).HasMaxLength(20);
            entity.Property(technician => technician.HireDate).IsRequired();

            /* Relations */

            entity.HasOne(technician => technician.User)
                .WithOne(user => user.Technician)
                .HasForeignKey<Technician>(technician => technician.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(technician => technician.UserId).IsUnique();
        });

        modelBuilder.Entity<TechnicianSkill>(entity =>
        {
            entity.HasKey(ts => new { ts.TechnicianId, ts.SkillId });

            /* Relations - TS belongs to one Technician */
            entity.HasOne(ts => ts.Technician)
                .WithMany(technician => technician.TechnicianSkills)
                .HasForeignKey(ts => ts.TechnicianId)
                .OnDelete(DeleteBehavior.Cascade);
            
            /* Relations - TS belongs to one skill*/
            entity.HasOne(ts => ts.Skill)
                .WithMany(skill => skill.TechnicianSkills)
                .HasForeignKey(ts => ts.SkillId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        /* Ticket configuration */

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(ticket => ticket.Id);
            entity.Property(ticket => ticket.Title).IsRequired().HasMaxLength(200);
            entity.Property(ticket => ticket.Description).IsRequired().HasMaxLength(2000);
            entity.Property(ticket => ticket.Priority).IsRequired();
            entity.Property(ticket => ticket.Status).IsRequired();
            entity.Property(ticket => ticket.ContactEmail).HasMaxLength(100);
            entity.Property(ticket => ticket.ContactName).HasMaxLength(100);
            entity.Property(ticket => ticket.ContactPhone).HasMaxLength(20);
                
            /* Relation - Ticket belongs to one Equipment*/
            entity.HasOne(ticket => ticket.Equipment)
                .WithMany(equipment => equipment.Tickets)
                .HasForeignKey(ticket => ticket.EquipmentId)
                .OnDelete(DeleteBehavior.Restrict);
                
            /* Relations - Ticket reported by one ApplicationUser */
            entity.HasOne(ticket => ticket.ReportedBy)
                .WithMany(user => user.ReportedTickets)
                .HasForeignKey(ticket => ticket.ReportedById)
                .OnDelete(DeleteBehavior.SetNull);
                
            /* Relations-  Ticket assigned to one Technician */
            entity.HasOne(ticket => ticket.AssignedTo)
                .WithMany(technician => technician.AssignedTickets)
                .HasForeignKey(ticket => ticket.AssignedToId)
                .OnDelete(DeleteBehavior.SetNull);
                
            /* Relation - Ticket updated by one ApplicationUser */
            entity.HasOne(ticket => ticket.UpdatedBy)
                .WithMany(user => user.UpdatedTickets)
                .HasForeignKey(ticket => ticket.UpdatedById)
                .OnDelete(DeleteBehavior.SetNull); 
        });
        
        /* TicketComment configuration */
        modelBuilder.Entity<TicketComment>(entity =>
        {
            entity.HasKey(tc => tc.Id);
            entity.Property(tc => tc.CommentText).IsRequired().HasMaxLength(2000);

            /* Relations - TC belongs to one ticket only */
            entity.HasOne(tc => tc.Ticket)
                .WithMany(ticket => ticket.Comments)
                .HasForeignKey(tc => tc.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            /* Relations - TC written by one ApplicationUser*/
            entity.HasOne(tc => tc.User)
                .WithMany(user => user.Comments)
                .HasForeignKey(tc => tc.UserId)
                .OnDelete(DeleteBehavior.Restrict);

        });
        
        /* MaintenanceSchedule configuration */
        modelBuilder.Entity<MaintenanceSchedule>(entity =>
        {
            entity.HasKey(ms => ms.Id);
            entity.Property(ms => ms.TaskDescription).IsRequired().HasMaxLength(500);
            entity.Property(ms => ms.IntervalDays).IsRequired();
            entity.Property(ms => ms.NextDue).IsRequired();
            entity.Property(ms => ms.IsActive).IsRequired();

            /* Relations - MaintenanceSchedukle belongs to one equipment*/
            entity.HasOne(ms => ms.Equipment)
                .WithMany(equipment => equipment.MaintenanceSchedules)
                .HasForeignKey(ms => ms.EquipmentId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}