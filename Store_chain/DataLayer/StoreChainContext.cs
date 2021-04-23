using Microsoft.EntityFrameworkCore;
using Store_chain.Data;
using Store_chain.Model;
using Store_chain.Models;

namespace Store_chain.DataLayer
{
    public partial class StoreChainContext : DbContext
    {
        public StoreChainContext()
        {
        }

        public StoreChainContext(DbContextOptions<StoreChainContext> options)
            : base(options)
        {
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder options)
        //    => options.UseSqlite("data source=localhost\\SQLEXPRESS;initial catalog=Store Chain;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework");

        public virtual DbSet<Customers> Customers { get; set; }
        public virtual DbSet<Department> Department { get; set; }
        public virtual DbSet<Transactions> Transactions { get; set; }
        public virtual DbSet<Products> Products { get; set; }
        public virtual DbSet<Suppliers> Suppliers { get; set; }
        public virtual DbSet<ProductMinQuantity> ProductMinQuantity { get; set; }
        public virtual DbSet<CentralStoreCapital> CentralStoreCapital { get; set; }
    }
}
