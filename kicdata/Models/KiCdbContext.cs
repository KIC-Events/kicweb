using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;
using kicdata.Models;

namespace KiCData.Models
{
    public class KiCdbContext : DbContext
    {
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<Volunteer> Volunteers { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Presenter> Presenters { get; set; }
        public DbSet<Presentation> Presentations { get; set; }

        public string DbPath { get; }

        public KiCdbContext()
        {
            DbPath = string.Empty;
        }
    }
}
