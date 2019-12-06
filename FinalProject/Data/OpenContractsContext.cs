using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinalProject.Models;

namespace FinalProject.Data
{
    public class OpenContractsContext : DbContext
    {
        public OpenContractsContext(DbContextOptions<OpenContractsContext> options) : base(options) { }


        public DbSet<OpenContract> OpenContracts { get; set; }


    }
}
