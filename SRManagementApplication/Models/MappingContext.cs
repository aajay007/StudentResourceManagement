using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SRManagementApplication.Models
{
    public class MappingContext : DbContext
    {
        public MappingContext(DbContextOptions<MappingContext> options)
      : base(options)
        {
        }

        public DbSet<Student> Student { get; set; }

        public DbSet<Resource> Resource { get; set; }
    }
}
