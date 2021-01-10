using Store_chain.Model;
using Store_chain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Store_chain.Data;
using Store_chain.DataLayer;
using Store_chain.Enums;
using TransactionManager = Store_chain.Data.TransactionManager;

namespace Store_chain.HelperMethods
{
    public class ActionsHelper
    {
        private readonly StoreChainContext _context;
        public ActionsHelper(StoreChainContext context)
        {
            _context = context;
        }

        /// <summary>
        /// It updates the supplier with their due money.
        /// Updates product with quantity in storage.
        /// Updates the last row of table store to be the capital in the store.
        /// </summary>
        /// <param name="supplierKey"></param>
        /// <param name="product"></param>
        /// <param name="productQuantity"></param>
        /// <returns></returns>
        public async Task Supply(int supplierKey, Products product, int productQuantity)
        {
            var timeOfTransaction = DateTime.Now;
            var transactionManager = new TransactionManager(_context);

            // the transaction of paying the supplier
            var transaction = new Transactions
            {
                RecipientKey = supplierKey,
                ProviderKey = 0, // 0 is the shop
                DateOfTransaction = timeOfTransaction,
                ProductKey = product.Id,
                ProductQuantity = productQuantity,
                ErrorText = string.Empty,
                State = (int)StateEnum.UndeterminedState,
                Type = "Bought from supplier"
            };

            await using (var t = _context.Database.BeginTransaction())
            {

                try
                {
                    // value of the product batch
                    var boughtValue = product.BoughtFromSuppliersCost * productQuantity;

                    // update the suppliers due payment
                    UpdateSuppliersDue(supplierKey, boughtValue);

                    // update the storage whe just bought
                    UpdateProductInStorage(product, supplierKey, productQuantity);

                    transaction.Capital = boughtValue;

                    transaction.State = (int)StateEnum.OkState;
                    transactionManager.AddTransaction(transaction);

                    var idOfTransaction = transactionManager.GetTransaction(transaction).Id;

                    var storeManager = new StoreManager(_context);

                    storeManager.CreateStoreRow(boughtValue, idOfTransaction, StoreCalculationEnum.Subtraction);
                }
                catch (Exception error)
                {
                    // transaction is updated to show the error message
                    transaction.ErrorText = error.Message;
                    transaction.State = (int)StateEnum.ErrorState;
                    transactionManager.AddTransaction(transaction);

                    throw error;
                }

                _context.SaveChanges();
                await t.CommitAsync();
            }
        }

        /// <summary>
        /// Supplier is updated with the money due.
        /// </summary>
        /// <param name="supplierKey"></param>
        /// <param name="boughtValue"></param>
        public void UpdateSuppliersDue(int supplierKey, decimal boughtValue)
        {
            var supplier = _context.Suppliers.FirstOrDefault(x => x.Id == supplierKey);
            if (supplier == null)
                throw new Exception("The specified supplier was not found");

            supplier.PaymentDue += boughtValue;

            _context.Suppliers.Update(supplier);
        }

        /// <summary>
        /// Product's quantity in storage is updated
        /// </summary>
        /// <param name="product"></param>
        /// <param name="supplierKey"></param>
        /// <param name="productQuantity"></param>
        public void UpdateProductInStorage(Products product, int supplierKey, int productQuantity)
        {
            if (product.SupplierKey != supplierKey)
                throw new Exception("The specified supplier does not contain the product");
            
            product.QuantityInStorage += productQuantity;

            _context.Products.Update(product);
            _context.SaveChanges();
        }

        /// <summary>
        /// Add products displayed in their respective department in the store
        /// </summary>
        /// <param name="productKey"></param>
        /// <param name="numToBeDisplayed"></param>
        /// <param name="department"></param>
        /// <returns></returns>
        public async Task Display(int productKey, int numToBeDisplayed, int department)
        {
            //Check if product passed is in the database
            var toBeSavedProduct = _context.Products.FirstOrDefault(x => x.Id == productKey);

            if (toBeSavedProduct == null)
                throw new Exception("No such Product in the database");
            await using (var transaction = _context.Database.BeginTransaction())
            {
                // up the number of displayed 
                toBeSavedProduct.QuantityInDisplay += numToBeDisplayed;

                toBeSavedProduct.QuantityInStorage -= numToBeDisplayed;

                // check if connected with the department 
                var productAlreadyInDepartment = _context.Department
                    .FirstOrDefault(x => x.Id == department && x.Prod_Id == toBeSavedProduct.Id);

                _context.Products.Update(toBeSavedProduct);

                // if the connection does not exist, create it
                if (productAlreadyInDepartment == null)
                {
                    toBeSavedProduct.Department = department;
                    toBeSavedProduct.DepartmentForeignId = department;
                    var newConn = new Department
                    {
                        DepartmentKey = department,
                        Description = toBeSavedProduct.Description,
                        Number = numToBeDisplayed,
                        State = toBeSavedProduct.MaxDisplay == numToBeDisplayed
                            ? (int)DepartmentProductState.Filled
                            : (int)DepartmentProductState.NeedFilling
                    };
                    _context.Department.Add(newConn);
                }
                // else update the number of products in display 
                else
                {
                    productAlreadyInDepartment.Number += numToBeDisplayed;

                    if (productAlreadyInDepartment.Number == toBeSavedProduct.MaxDisplay)
                        productAlreadyInDepartment.State = (int)DepartmentProductState.Filled;
                    else if (productAlreadyInDepartment.Number > toBeSavedProduct.MaxDisplay)
                        productAlreadyInDepartment.State = (int)DepartmentProductState.OverFilled;
                    else
                        productAlreadyInDepartment.State = (int)DepartmentProductState.NeedFilling;

                    _context.Department.Update(productAlreadyInDepartment);
                }

                _context.SaveChanges();
                await transaction.CommitAsync();
            }
        }

        public async Task Buy(Products product, Customers buyer)
        {
            // Get the value for all the products the customer is buying 
            var summedValue = product.SoldToCustomersCost * product.TransactionQuantity;

            if (summedValue == 0)
                throw new Exception("No Products bought");

            var timeOfTransaction = DateTime.Now;

            if (buyer.Capital - summedValue <= 0)
                throw new Exception("Customer Does not have the capital required to the transaction");

            var transactionManager = new TransactionManager(_context);

            // begin the transaction 
            await using (var transaction = _context.Database.BeginTransaction())
            {
                buyer.Capital -= summedValue;

                // Create a new transaction about
                // who bought
                // what time did the transaction take place
                // the amount the customer is buying
                var customerFullTransaction =
                    transactionManager.AddTransaction(new Transactions
                    {
                        RecipientKey = 0,
                        ProviderKey = buyer.Id,
                        ProductKey = product.Id,
                        DateOfTransaction = timeOfTransaction,
                        State = (int)StateEnum.UndeterminedState,
                        Capital = summedValue,
                        ErrorText = string.Empty,
                        Type = "Sold to customer"
                    });

                if (customerFullTransaction == null)
                    throw new Exception("transaction manager has encountered a problem");
                try
                {
                    // update the customer with the new capital after paying
                    _context.Customers.Update(buyer);

                    await UpdateProductInDisplay(product);

                    // create another entry in the transactions, with all the info needed to be recognized
                    // subset of the major 
                    customerFullTransaction.ProductQuantity = product.TransactionQuantity;
                    customerFullTransaction.ErrorText = "OK!";
                    customerFullTransaction.State = (int)StateEnum.OkState;

                    var storeManager = new StoreManager(_context);

                    storeManager.CreateStoreRow(summedValue, customerFullTransaction.Id, StoreCalculationEnum.Addition);
                }
                catch (Exception error)
                {
                    customerFullTransaction.State = (int)StateEnum.ErrorState;
                    customerFullTransaction.ErrorText = error.Message;
                    throw error;
                }

                await CheckIfNeedReSupply(product.toListFromOne());
                
                _context.Transactions.Update(customerFullTransaction);
                _context.SaveChanges();
                await transaction.CommitAsync();
            }
        }

        private async Task CheckIfNeedReSupply(List<Products> products)
        {
            var productsFromDepartments =
                (from product in products
                 join minQuantity in _context.ProductMinQuantity
                     on product.Id equals minQuantity.id
                 where product.QuantityInStorage < minQuantity.MinStorage
                 select new
                 {
                     product,
                     QuantityToBeSupplied = minQuantity.MinStorage - product.QuantityInStorage
                 }).ToList();

            if (productsFromDepartments.Any())
                await Task.Run(() => productsFromDepartments
                    .ForEach(async x => await Supply(x.product.SupplierKey, x.product, x.QuantityToBeSupplied)));
        }

        public async Task UpdateProductInDisplay(Products productBought)
        {
            var departmentConnection = _context.Department.FirstOrDefault(x => x.Prod_Id == productBought.Id);

            if (departmentConnection == null)
                throw new Exception("Connection in department by product id was not found");
            departmentConnection.Number -= productBought.TransactionQuantity;

            productBought.QuantityInDisplay -= productBought.TransactionQuantity;

            _context.SaveChanges();
        }

        public void CheckValidityOfBuy(BuyActionClass buyClass)
        {
            if (buyClass.CustomerKey == 0)
                throw new Exception("Please select a customer");
            if (buyClass.ProductKey == 0)
                throw new Exception("Please select a product");
            if (buyClass.Quantity == 0)
                throw new Exception("Please give an amount of product you want to buy");
        }

        public List<Products> BringAllProducts()
        {
            try
            {
                var products = _context.Products.Select(x => x).ToList();

                return products;
            }
            catch (Exception err)
            {
                throw err;
            }

        }
    }
}
