using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Store_chain.Models;

namespace Store_chain.Data
{
    public class Store_chainContext : DbContext
    {
        public Store_chainContext (DbContextOptions<Store_chainContext> options)
            : base(options)
        {
        }

        public DbSet<Store_chain.Models.Product> Product { get; set; }
    }
}
