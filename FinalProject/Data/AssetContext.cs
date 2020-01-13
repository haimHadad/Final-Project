using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinalProject.Models;

namespace FinalProject.Data
{
    public class AssetContext : DbContext
    {
        //database class (API) to communicate with db (assets table = Land registration)
        public AssetContext(DbContextOptions<AssetContext> options) : base(options) { }
        public DbSet<Asset> Assets { get; set; }
    }
}
