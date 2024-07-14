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

                optionsBuilder.UseMySql(builder.ConnectionString, ServerVersion.AutoDetect(builder.ConnectionString));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Member>()
                .UseTptMappingStrategy();
            modelBuilder.HasDefaultSchema("Dev");
        }
    }
}
