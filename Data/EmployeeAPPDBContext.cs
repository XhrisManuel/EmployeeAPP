using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EmployeeAPP.Models;

namespace EmployeeAPP.Data
{
    public class EmployeeAPPDBContext : DbContext
    {
        public EmployeeAPPDBContext (DbContextOptions<EmployeeAPPDBContext> options)
            : base(options)
        {
        }

        public DbSet<EmployeeAPP.Models.Employee> Employee { get; set; } = default!;
    }
}
