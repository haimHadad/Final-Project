using FinalProject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Data
{
    public class AssetsInContractContext : DbContext
    {
        public AssetsInContractContext(DbContextOptions<AssetsInContractContext> options) : base(options) { }
        //database class (API) to communicate with db (open contracts)

        public DbSet<AssetInContract> AssetsInContract { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AssetInContract>()
                .HasKey(c => new { c.AssetID, c.ContractAddress });
        }
    }
}
