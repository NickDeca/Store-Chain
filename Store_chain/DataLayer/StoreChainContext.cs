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
        public virtual DbSet<Employees> Employees { get; set; }

        public virtual DbSet<Department> Departments { get; set; }

        //TODO remove ProductDepartment
        //public virtual DbSet<ProductDepartment> ProductDepartments { get; set; }
        public virtual DbSet<Transactions> transactionTable { get; set; }
        public virtual DbSet<Products> Products { get; set; }
        //public virtual DbSet<StoreDepartments> StoreDepartments { get; set; }
        public virtual DbSet<Suppliers> Suppliers { get; set; }
        public virtual DbSet<ProductMinQuantity> MinQuantities { get; set; }
        public virtual DbSet<CentralStoreCapital> Store { get; set; }
    }
}
