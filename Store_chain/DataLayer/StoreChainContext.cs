using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Store_chain.Data;
using Store_chain.Models;

namespace Store_chain.Model
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

        public virtual DbSet<Customers> Customers { get; set; }
        public virtual DbSet<Employees> Employees { get; set; }
        public virtual DbSet<ProductCategories> ProductDepartments { get; set; }
        public virtual DbSet<Transactions> transactionTable { get; set; }
        public virtual DbSet<Products> Products { get; set; }
        public virtual DbSet<StoreDepartments> StoreDepartments { get; set; }
        public virtual DbSet<Suppliers> Suppliers { get; set; }
        public virtual DbSet<ProductMinQuantity> MinQuantities { get; set; }
        public virtual DbSet<CentralStoreCapital> Store { get; set; }
    }
}
