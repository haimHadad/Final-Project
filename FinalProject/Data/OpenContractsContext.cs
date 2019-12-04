using FinalProject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Data
{
    public class OpenContractsContext : DbContext
    {
        public OpenContractsContext(DbContextOptions<AssetContext> options) : base(options) { }


        public DbSet<OpenContract> OpenContractContracts { get; set; }

 
    }
}
