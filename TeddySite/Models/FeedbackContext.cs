using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace TeddySite.Models
{
    public class FeedbackContext : DbContext
    {
        public FeedbackContext()
            : base("DefaultConnection")
        {
        }
        public DbSet<FeedbackEntry> Entries { get; set; }
    }
}