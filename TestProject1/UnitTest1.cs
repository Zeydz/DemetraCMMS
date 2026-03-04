using Xunit;
using dotnet_projektuppgift.Models.Enums;
using dotnet_projektuppgift.Models.Entities;
using System;

namespace TestProject1
{
    /* Basic tests for CI/CD pipeline*/
    public class UnitTest1
    {

        /*Test: Ticket priority enum values are correct*/

        [Fact]
        public void TicketPriority_ShouldHaveCorrectValues()
        {

            var low = TicketPriority.Low;
            var medium = TicketPriority.Medium;
            var high = TicketPriority.High;
            var critical = TicketPriority.Critical;

            /*Assert*/
            Assert.Equal(0, (int)low);
            Assert.Equal(1, (int)medium);
            Assert.Equal(2, (int)high);
            Assert.Equal(3, (int)critical);
        }


        /*Test: Ticket status enum values are correct*/

        [Fact]
        public void TicketStatus_ShouldHaveCorrectValues()
        {
            var newStatus = TicketStatus.New;
            var assigned = TicketStatus.Assigned;
            var inProgress = TicketStatus.InProgress;
            var resolved = TicketStatus.Resolved;
            var closed = TicketStatus.Closed;

            /*Assert*/
            Assert.Equal(0, (int)newStatus);
            Assert.Equal(1, (int)assigned);
            Assert.Equal(2, (int)inProgress);
            Assert.Equal(3, (int)resolved);
            Assert.Equal(4, (int)closed);
        }


        /*Test: Equipment status enum values are correct*/
        [Fact]
        public void EquipmentStatus_ShouldHaveCorrectValues()
        {

            var operational = EquipmentStatus.Operational;
            var underMaintenance = EquipmentStatus.UnderMaintenance;
            var outOfService = EquipmentStatus.OutOfService;

            /*Assert*/
            Assert.Equal(0, (int)operational);
            Assert.Equal(1, (int)underMaintenance);
            Assert.Equal(2, (int)outOfService);
        }


        /*Test: Location entity initialization*/
        [Fact]
        public void Location_ShouldInitializeCorrectly()
        {

            var location = new Location
            {
                Id = 1,
                Name = "Building A",
                Building = "123 Main St",
                Floor = "Floor 3"
            };

            /*Assert*/
            Assert.Equal(1, location.Id);
            Assert.Equal("Building A", location.Name);
            Assert.Equal("123 Main St", location.Building);
            Assert.Equal("Floor 3", location.Floor);
        }


        /*Test: Skill entity initialization*/
        [Fact]
        public void Skill_ShouldInitializeCorrectly()
        {

            var skill = new Skill
            {
                Id = 1,
                Name = "HVAC",
                Description = "Heating, Ventilation, and Air Conditioning"
            };

            /*Assert*/
            Assert.Equal(1, skill.Id);
            Assert.Equal("HVAC", skill.Name);
            Assert.Equal("Heating, Ventilation, and Air Conditioning", skill.Description);
        }


        /*Test: Ticket entity default values*/

        [Fact]
        public void Ticket_ShouldHaveCorrectDefaults()
        {

            var ticket = new Ticket
            {
                Title = "Test Ticket",
                Description = "Test Description",
                Priority = TicketPriority.Medium,
                Status = TicketStatus.New
            };

            /*Assert*/
            Assert.Equal("Test Ticket", ticket.Title);
            Assert.Equal("Test Description", ticket.Description);
            Assert.Equal(TicketPriority.Medium, ticket.Priority);
            Assert.Equal(TicketStatus.New, ticket.Status);
        }


        /*Test: Equipment entity initialization with relationships*/
        [Fact]
        public void Equipment_ShouldInitializeWithLocation()
        {

            var location = new Location { Id = 1, Name = "Building A" };
            

            var equipment = new Equipment
            {
                Id = 1,
                Name = "HVAC Unit",
                Status = EquipmentStatus.Operational,
                LocationId = location.Id,
                Location = location
            };

            /*Assert*/
            Assert.Equal(1, equipment.Id);
            Assert.Equal("HVAC Unit", equipment.Name);
            Assert.Equal(EquipmentStatus.Operational, equipment.Status);
            Assert.Equal(location.Id, equipment.LocationId);
            Assert.NotNull(equipment.Location);
            Assert.Equal("Building A", equipment.Location.Name);
        }


        /*Test: Technician entity basic properties*/
        [Fact]
        public void Technician_ShouldInitializeCorrectly()
        {

            var hireDate = DateTime.Today;
            var technician = new Technician
            {
                Id = 1,
                HireDate = hireDate,
                IsActive = true
            };

            /*Assert*/
            Assert.Equal(1, technician.Id);
            Assert.Equal(hireDate, technician.HireDate);
            Assert.True(technician.IsActive);
        }
        
        /*Test: Ticket priority comparison */
        /*InlineData allows to test multiple scenarios at the same time*/
        [Theory]
        [InlineData(TicketPriority.Low, TicketPriority.Medium, true)]
        [InlineData(TicketPriority.Medium, TicketPriority.High, true)]
        [InlineData(TicketPriority.High, TicketPriority.Critical, true)]
        [InlineData(TicketPriority.Critical, TicketPriority.Low, false)]
        public void TicketPriority_ComparisonShouldWork(TicketPriority lower, TicketPriority higher, bool expectedLessThan)
        {

            var result = lower < higher;


            Assert.Equal(expectedLessThan, result);
        }
        
        /*Test: Ticket status progression logic*/
        [Theory]
        [InlineData(TicketStatus.New, TicketStatus.Assigned, true)]
        [InlineData(TicketStatus.Assigned, TicketStatus.InProgress, true)]
        [InlineData(TicketStatus.InProgress, TicketStatus.Resolved, true)]
        [InlineData(TicketStatus.Resolved, TicketStatus.Closed, true)]
        [InlineData(TicketStatus.Closed, TicketStatus.New, false)]
        public void TicketStatus_ProgressionShouldWork(TicketStatus current, TicketStatus next, bool expectedValid)
        {

            var result = next >= current;
            
            Assert.Equal(expectedValid, result);
        }
    }
}