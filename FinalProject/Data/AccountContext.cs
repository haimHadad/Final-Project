using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinalProject.Models;

namespace FinalProject.Data
{
    public class AccountContext : DbContext
    {
        public AccountContext(DbContextOptions<AccountContext> options) : base(options) { }
        public DbSet<Asset> assets { get; set; }
        public DbSet<OpenContract> recievedOpenContracts { get; set; }

        //public DbSet<ClosedContract> sentOpenContracts { get; set; }
    }
}
