using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialBrothers.Models
{
    public class SocialBrothersContext : DbContext
    {
        public SocialBrothersContext(DbContextOptions<SocialBrothersContext> options)
            : base(options)
        {
        }

        public DbSet<Adress> TodoItems { get; set; }
    }
}
