using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

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
        public virtual DbSet<ProductCategories> ProductCategories { get; set; }
        public virtual DbSet<Products> Products { get; set; }
        public virtual DbSet<StoreDepartments> StoreDepartments { get; set; }
        public virtual DbSet<Suppliers> Suppliers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json").Build();
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("Store_chainContext"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customers>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Capital).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Description)
                    .HasMaxLength(20)
                    .IsFixedLength();

                entity.Property(e => e.FirstName)
                    .HasColumnName("First_Name")
                    .HasMaxLength(20)
                    .IsFixedLength();

                entity.Property(e => e.LastName)
                    .HasColumnName("Last_Name")
                    .HasMaxLength(20)
                    .IsFixedLength();
            });

            modelBuilder.Entity<Employees>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.FirstName)
                    .HasColumnName("First_Name")
                    .HasMaxLength(20)
                    .IsFixedLength();

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.LastName)
                    .HasColumnName("Last_Name")
                    .HasMaxLength(20)
                    .IsFixedLength();
            });

            modelBuilder.Entity<ProductCategories>(entity =>
            {
                entity.ToTable("Product_Categories");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description)
                    .HasMaxLength(20)
                    .IsFixedLength();
            });

            modelBuilder.Entity<Products>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.IsDisplay).HasColumnName("isDisplay");

                entity.Property(e => e.SupplierKey).HasColumnName("Supplier_Key");
            });

            modelBuilder.Entity<StoreDepartments>(entity =>
            {
                entity.ToTable("Store_Departments");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description)
                    .HasMaxLength(20)
                    .IsFixedLength();
            });

            modelBuilder.Entity<Suppliers>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description)
                    .HasMaxLength(20)
                    .IsFixedLength();

                entity.Property(e => e.Name)
                    .HasMaxLength(20)
                    .IsFixedLength();

                entity.Property(e => e.PaymentDue)
                    .HasColumnName("Payment_Due")
                    .HasColumnType("decimal(18, 2)");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
