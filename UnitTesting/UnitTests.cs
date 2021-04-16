using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Store_chain.Data;
using Store_chain.DataLayer;
using Store_chain.Exceptions;
using Store_chain.HelperMethods;
using Store_chain.Model;
using Store_chain.Models;

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
        public void Test_Products_Retrival_Not_Null()
        {
            // Arrange
                        
            // Act
            var products = _context.Products.ToList();

            //Assert
            CollectionAssert.AllItemsAreNotNull(products);
        }

        [Test]
        public void UnitTestDepartments()
        {
            // Arrange
            var departments = _context.Department.ToList();

            // Act

            var products = departments.SelectMany(x => x.Products).ToList();

            // Assert
            Assert.That(products != null);
        }

        [Test]
        public void CheckValidityOfBuy_Valid_Class()
        {
            // Arrange
            var helper = new ActionsHelper(_context);
            var buyClass = new BuyActionClass
            {
                CustomerKey = 1,
                ProductKey = 1,
                Quantity = 10
            };

            // Act

            // Assert
            Assert.DoesNotThrow(() => helper.CheckValidityOfBuy(buyClass));
        }

        [Test]
        public void CheckValidityOfBuy_NonValid_Class()
        {
            // Arrange
            var helper = new ActionsHelper(_context);
            var buyClass = new BuyActionClass
            {
                CustomerKey = 0,
                ProductKey = 1,
                Quantity = 10
            };

            // Act

            // Assert
            Assert.Throws<ValidityException>(() => helper.CheckValidityOfBuy(buyClass));
        }

        [Test]
        public void UpdateSuppliersDue_NoSupplier()
        {
            // Arrange
            var supplierId = 100;
            var boughtValue = 1.00m;
            var helper = new ActionsHelper(_context);
            var exceptionExpected = new ActionsException("The specified supplier was not found", null, null, supplierId);

            // Act
            try
            {
                helper.UpdateSuppliersDue(supplierId, boughtValue);
            }
            catch (ActionsException ae)
            {
                // Assert
                Assert.IsNull(ae.CustomerId);
                Assert.IsNull(ae.ProductId);
                Assert.AreEqual(ae.SupplierId, exceptionExpected.SupplierId);
                StringAssert.AreEqualIgnoringCase(ae.Message, exceptionExpected.Message);
            }
            catch (Exception e)
            {
                // Assert
                Assert.Equals(e, exceptionExpected);
            }
        }

        [Test]
        public void UpdateSuppliersDue_Supplier()
        {
            // Arrange
            var supplierId = 1;
            var boughtValue = 1.00m;
            var helper = new ActionsHelper(_context);
            var supplierBefore = _context.Suppliers.Find(supplierId);
            var expectedValue = supplierBefore.PaymentDue + 1.00m;

            // Act
            helper.UpdateSuppliersDue(supplierId, boughtValue);

            var supplierAfter = _context.Suppliers.Find(supplierId);

            // Assert
            Assert.AreEqual(supplierAfter.PaymentDue, expectedValue);
        }

        [Test]
        public void UpdateProductInStorage_No_Product_Found()
        {
            // Arrange
            var product = _context.Products.Find(1);
            var supplierId = 2;
            var productQuantity = 100;
            var helper = new ActionsHelper(_context);
            var exceptionExpected = new ActionsException("The specified supplier does not contain the product", null, product.Id, supplierId);

            // Act
            try
            {
                helper.UpdateProductInStorage(product, supplierId, productQuantity);

            }
            catch (ActionsException ae)
            {
                // Assert
                Assert.IsNull(ae.CustomerId);
                Assert.Equals(ae.ProductId, product.Id);
                Assert.AreEqual(ae.SupplierId, exceptionExpected.SupplierId);
                StringAssert.AreEqualIgnoringCase(ae.Message, exceptionExpected.Message);
            }
            catch (Exception e)
            {
                // Assert
                Assert.Equals(e, exceptionExpected);
            }

        }

        [Test]
        public void UpdateProductInStorage_Product_Updated()
        {
            // Arrange
            var productBefore = _context.Products.Find(1);
            var supplierId = 2;
            var productQuantity = 1;
            var helper = new ActionsHelper(_context);
            var expectedQuantity = productBefore.QuantityInStorage + productQuantity;

            // Act
            helper.UpdateProductInStorage(productBefore, supplierId, productQuantity);
            var productAfter = _context.Products.Find(1);

            // Assert
            Assert.Equals(expectedQuantity, productAfter.QuantityInStorage);
        }

        [Test]
        public async Task UpdateProductInDisplay_Department_Not_FoundAsync()
        {
            // Arrange
            var helper = new ActionsHelper(_context);
            var productId = 11;
            var product = _context.Products.Find(productId);
            var quantity = 1;
            var departmentConnection = _context.Department.FirstOrDefault(x => x.Prod_Id == 1);
            var exceptionExpected = new ConnectionExceptions("Connection in department by product id was not found", productId);

            try
            {
                // Act
                await helper.UpdateProductInDisplay(product, quantity);
            }
            catch (ConnectionExceptions ce)
            {
                // Assert
                Assert.AreEqual(ce.ProductId, productId);
                StringAssert.AreEqualIgnoringCase(ce.Message, exceptionExpected.Message);

            }
            catch (Exception e)
            {
                // Assert
                Assert.Equals(e, exceptionExpected);
            }

        }

        [Test]
        public async Task UpdateProductInDisplay_Updated_DisplayAsync()
        {
            // Arrange
            var helper = new ActionsHelper(_context);
            var productId = 1;
            var productBefore = _context.Products.Find(productId);
            var departmentConnectionBefore = _context.Department.FirstOrDefault(x => x.Prod_Id == productId);
            var quantity = 1;

            var productExpectedQuantityDisplay = productBefore.QuantityInDisplay - quantity;
            var departmentnumberExpected = departmentConnectionBefore.Number - quantity;

            // Act
            await helper.UpdateProductInDisplay(productBefore, quantity);
            var productAfter = _context.Products.Find(productId);
            var departmentConnectionAfter = _context.Department.FirstOrDefault(x => x.Prod_Id == productId);

            // Assert
            Assert.AreEqual(productExpectedQuantityDisplay, productAfter.QuantityInDisplay);
            Assert.AreEqual(departmentnumberExpected, departmentConnectionAfter.Number);
        }

        [Test]
        public void CheckNeedForResupply_UnitTest()
        {
            // Arrange
            var helper = new ActionsHelper(_context);
            var listProductIds = new List<int> { 1, 2, 3, 4, 11, 15 };
            var products = _context.Products.Where(x => listProductIds.Contains(x.Id));

            var listExpectedIds = new List<int> { 2, 3, 15 };

            // Act
            var productsNeedingResupply = helper.CheckNeedForResupply(products);
            var resultedProductsForResupply = productsNeedingResupply.Select(x => x.Item1.Id).OrderBy(x => x).ToList();

            // Assert
            CollectionAssert.AreEqual(listExpectedIds, resultedProductsForResupply);
        }

        [Test]
        public void CheckNeedForResupply_NoNeed_For_Resupply()
        {
            // Arrange
            var helper = new ActionsHelper(_context);
            var listProductIds = new List<int> { 1, 4, 11 };
            var products = _context.Products.Where(x => listProductIds.Contains(x.Id));

            // Act
            var productsNeedingResupply = helper.CheckNeedForResupply(products);

            // Assert
            CollectionAssert.IsEmpty(productsNeedingResupply);
        }

        [Test]
        public async Task Creation_Test_CustomersAsync()
        {
            // Arrange
            var customer = new Customers
            {
                Capital = 50.11m,
                Description = "CustomerNew",
                FirstName = "Customer",
                LastName = "New"                
            };
            var manager = new CustomerManager(_context);

            // Act
            await manager.CreateCustomer(customer);
            var customerCreated = _context.Customers.OrderByDescending(x => x.Id).FirstOrDefault();

            // Assert
            Assert.AreEqual(customer.Capital, customerCreated.Capital);
            StringAssert.AreEqualIgnoringCase(customer.FirstName, customerCreated.FirstName);
            StringAssert.AreEqualIgnoringCase(customer.LastName, customerCreated.LastName);
            StringAssert.AreEqualIgnoringCase(customer.Description, customerCreated.Description);
        }

        [Test]
        public void UnitTest()
        {
            // Arrange

            // Act

            // Assert
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
    }
}
