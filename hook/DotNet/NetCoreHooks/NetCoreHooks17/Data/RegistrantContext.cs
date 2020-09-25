using Microsoft.EntityFrameworkCore;
using NetCoreHooks.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreHooks.Data
{
    public class RegistrantContext : DbContext
    {
        public DbSet<Registrant> Registrants { get; set; }
        public RegistrantContext(DbContextOptions<RegistrantContext> options)
       : base(options)
        { }

        
    }
}
