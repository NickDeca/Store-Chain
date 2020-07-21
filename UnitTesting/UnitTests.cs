using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Store_chain.DataLayer;

namespace UnitTesting
{
    [TestFixture]
    public class UnitTests
    {
        private StoreChainContext _context { get; set; }

        [SetUp]
        public void Setup()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            var optionsBuilder = new DbContextOptionsBuilder<StoreChainContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("Store_chainContext"));
            _context = new StoreChainContext(optionsBuilder.Options);
        }

        [Test]
        public void UnitTest1()
        {
            // Arrange

            
            // Act
            var products = _context.Products.ToList();

            //Assert
            CollectionAssert.AllItemsAreNotNull(products);
        }
        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
    }
}
