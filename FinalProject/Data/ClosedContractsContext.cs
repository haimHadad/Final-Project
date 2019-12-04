using FinalProject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Data
{
    public class ClosedContractsContext : DbContext
    {
        public ClosedContractsContext(DbContextOptions<AssetContext> options) : base(options) { }


        public DbSet<ClosedContract> ClosedContracts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClosedContract>()
                .HasKey(c => new { c.AssetID, c.ContractAddress });
        }
    }
}
