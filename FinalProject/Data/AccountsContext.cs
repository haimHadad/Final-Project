using FinalProject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Data
{
    public class AccountsContext : DbContext
    {
        //database class (API) to communicate with db (accounts table)
            public AccountsContext(DbContextOptions<AccountsContext> options) : base(options) { }
            public DbSet<AccountID> Accounts { get; set; }
        

    }
}
