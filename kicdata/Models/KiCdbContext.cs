using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;
using KiCData.Models;
using Microsoft.Extensions.Configuration;

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

        /// <summary>
        /// Initializes a new instance of the KiCdbContext class with the given IConfigurationRoot.
        /// </summary>
        /// <param name="config">An IConfigurationRoot object which must expose the connection string for the KiCdbContext.</param>
        public KiCdbContext(IConfigurationRoot config)
        {
            _config = config;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql(_config["ConnectionString"], ServerVersion.AutoDetect(_config["ConnectionString"]));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Member>()
                .UseTptMappingStrategy();
        }
    }
}
