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
        public DbSet<WaitList> WaitList { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<PendingVolunteer> PendingVolunteers { get; set; }



        public KiCdbContext(DbContextOptions<KiCdbContext> options) : base(options)
        {

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
            /*
            //modelBuilder.HasDefaultSchema("Dev");

            modelBuilder.Entity<Member>().HasData(new Member
                        {
                Id = 7725,
                FirstName = "John",
                LastName = "Doe",
                Email = "John.Doe@example.com",
                DateOfBirth = new DateOnly(1980, 1, 1),
                FetName = "JohnDoe",
                ClubId = 12345,
                PhoneNumber = "555-555-5555",
                PublicId = 54321,
                AdditionalInfo = "This is a test user.",
            });
            modelBuilder.Entity<Presenter>().HasData(new Presenter
            {
                Id = 1234,
                PublicName = "Test Presenter",
                Bio = "This is a test presenter.",
                LastAttended = new DateOnly(2021, 1, 1),
                Requests = "Test Requests",
                Fee = 100.00M,
                Details = "Test Details"
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
                PriceAvg = 5.00M,
                ImgPath = @"/wwwroot/images/Vendors/image01.jpg"
            });
            modelBuilder.Entity<Volunteer>().HasData(new Volunteer
            {
                Id = 1234,
                MemberId = 7725,
                Positions = new List<string> { "Test Position" },
                Details = "Test Details"
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
                Id = 1111,
                Name = "Test Event",
                StartDate = new DateOnly(2021, 1, 1),
                EndDate = new DateOnly(2021, 1, 2),
                VenueId = 12345,
                Description = "This is a test event."
            });
            modelBuilder.Entity<Ticket>().HasData(new Ticket
            {
                Id = 1234,
                EventId = 1111,
                Price = 10.00,
                Type = "Test Ticket",
                //Name = "Test Ticket",
                DatePurchased = new DateOnly(2021, 1, 1),
                StartDate = new DateOnly(2021, 1, 1),
                EndDate = new DateOnly(2021, 1, 2),
                IsComped = false
            });
            modelBuilder.Entity<Ticket>().HasData(new Ticket
            {
                Id = 12354,
                EventId = 1111,
                Price = 10.00,
                Type = "Test Ticket",
                //Name = "Test Ticket",
                DatePurchased = new DateOnly(2021, 1, 1),
                StartDate = new DateOnly(2021, 1, 1),
                EndDate = new DateOnly(2021, 1, 2),
                IsComped = false
            });
            modelBuilder.Entity<Presentation>().HasData(new Presentation
            {
                Id = 2222,
                Name = "Test Presentation",
                PresenterId = 1234,
                Description = "This is a test presentation.",
                EventId = 1111,
                ImgPath = @"/wwwroot/Presentations/image01.jpg"
            });
            modelBuilder.Entity<EventVendor>().HasData(new EventVendor
            {
                Id = 3333,
                EventId = 1111,
                VendorId = 1128
            });
            //modelBuilder.Entity<EventVolunteer>().HasData(new EventVolunteer
            //{
            //    Id = 3579,
            //    EventId = 1111,
            //    VolunteerId = 1234
            //});
            modelBuilder.Entity<Attendee>().HasData(new Attendee
            {
                Id = 2468,
                MemberId = 7725,
                TicketId = 1234,
                BadgeName = "RandomNessy",
                BackgroundChecked = true,
                RoomWaitListed = false,
                TicketWaitListed = false,
                RoomPreference = "Special",
                IsPaid = true
            });

            modelBuilder.Entity<Group>().HasData(new Group
            {
                Id = new Guid(),
                Name = "Admin",
                Description = "Admin Group"
            });

            //modelBuilder.Entity<Group>().HasData(new Group
            //{
            //    Id = new Guid(),
            //    Name = "Contributor",
            //    Description = "Vendor and Presenter Coordinator Group"
            //});

            modelBuilder.Entity<User>().HasData(new User
            {
                Id = new Guid(),
                MemberId = 7725,
                Username = "admin",
                Password = "password",
                
            });
            */
        }
    }
}
