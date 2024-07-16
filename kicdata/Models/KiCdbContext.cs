using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;
using KiCData.Models;
using Microsoft.Extensions.Configuration;
using System.Xml.Serialization;
using MySqlConnector;

namespace KiCData.Models
{
    public class KiCdbContext : DbContext
    {
        private IConfigurationRoot _config;

        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<Volunteer> Volunteers { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Presenter> Presenters { get; set; }
        public DbSet<Presentation> Presentations { get; set; }
        public DbSet<ClubMember> ClubMembers { get; set; }
        public DbSet<Attendee> Attendees { get; set; }
        public DbSet<EventVendor> EventVendors{ get; set; }
        public DbSet<EventVolunteer> EventVolunteers { get; set; }
        public DbSet<Staff> Staff { get; set; }
        public DbSet<Ticket> Ticket { get; set; }
        public DbSet<TicketComp> TicketComp { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Venue> Venue { get; set; }


        public KiCdbContext(IConfigurationRoot config)
        {
            _config = config;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder();
                builder.Server = _config["Database:Server"];
                builder.Port = 3306;
                builder.UserID = _config["Database:Username"];
                builder.Password = _config["Database:Password"];
                builder.Database = _config["Database:Database"];

                optionsBuilder.UseMySql(builder.ConnectionString, ServerVersion.AutoDetect(builder.ConnectionString));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Member>()
                .UseTptMappingStrategy();
            modelBuilder.HasDefaultSchema("Dev");
            modelBuilder.Entity<Member>().HasData(new Volunteer
                        {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Email = "John.Doe@example.com",
                DateOfBirth = new DateOnly(1980, 1, 1),
                FetName = "JohnDoe",
                ClubId = 12345,
                PhoneNumber = "555-555-5555",
                PublicId = 54321,
                AdditionalInfo = "This is a test user.",
                IsVendor = false,
                IsVolunteer = true,
                IsPresenter = false,
                IsStaff = false,
                Positions = new List<string> { "Test Position" },
                Details = "This is a test volunteer."
            });
            modelBuilder.Entity<Vendor>().HasData(new Vendor
            {
                Id = 1128,
                PublicName = "Test Vendor",
                Bio = "This is a test vendor.",
                LastAttended = new DateOnly(2021, 1, 1),
                MerchType = "Test Merch",
                PriceMin = 1.00M,
                PriceMax = 10.00M,
                PriceAvg = 5.00M
                });
            modelBuilder.Entity<Venue>().HasData(new Venue
            {
                Id = 12345,
                Name = "Test Venue",
                Address = "123 Test St.",
                City = "Test City",
                State = "TS",
                Capacity = 100
            });
            modelBuilder.Entity<Event>().HasData(new Event
            {
                Id = Guid.NewGuid(),
                Name = "Test Event",
                StartDate = new DateOnly(2021, 1, 1),
                EndDate = new DateOnly(2021, 1, 2),
                VenueId = 12345,
                Description = "This is a test event."
            });
        }
    }
}
