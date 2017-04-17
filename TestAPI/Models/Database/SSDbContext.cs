using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using ShowAndSellAPI.Models.Http;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace ShowAndSellAPI.Models.Database
{
    public class SSDbContext : DbContext
    {
        // Properties
        public DbSet<SSGroup> Groups { get; set; }
        public DbSet<SSUser> Users { get; set; }
        public DbSet<SSItem> Items { get; set; }
        public DbSet<SSImage> Images { get; set; }
        public DbSet<SSBookmark> Bookmarks { get; set; }
        public DbSet<SSMessage> Messages { get; set; }

        // Constructor
        public SSDbContext(DbContextOptions<SSDbContext> options) : base(options) { }
    }
}
