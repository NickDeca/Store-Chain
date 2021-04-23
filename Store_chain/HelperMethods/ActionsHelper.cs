using Store_chain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Store_chain.Data;
using Store_chain.DataLayer;
using Store_chain.Enums;
using Store_chain.Data.Managers;
using Store_chain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Store_chain.Model;

namespace Store_chain.HelperMethods
{
    public class ActionsHelper : IActionsHelper
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

            await using var t = _context.Database.BeginTransaction();

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
                await transactionManager.AddTransaction(transaction);

                var idOfTransaction = transactionManager.GetTransaction(transaction).Id;

                var storeManager = new StoreManager(_context);

                storeManager.CreateStoreRow(boughtValue, idOfTransaction, StoreCalculationEnum.Subtraction);
            }
            catch (Exception error)
            {
                // transaction is updated to show the error message
                transaction.ErrorText = error.Message;
                transaction.State = (int)StateEnum.ErrorState;
                await transactionManager .AddTransaction(transaction);

                throw error;
            }

            _context.SaveChanges();
            await t.CommitAsync();
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
                throw new ActionsException("The specified supplier was not found", null, null, supplierKey);

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
                throw new ActionsException("The specified supplier does not contain the product", null, product.Id, supplierKey);

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
            await using var transaction = _context.Database.BeginTransaction();
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

        public async Task Buy(Products product, Customers buyer, int transactionQuantity)
        {
            // Get the value for all the products the customer is buying 
            var summedValue = product.SoldToCustomersCost * transactionQuantity;

            if (summedValue == 0)
                throw new ActionsException("No Products bought");

            var timeOfTransaction = DateTime.Now;

            if (buyer.Capital - summedValue <= 0)
                throw new ActionsException("Customer Does not have the capital required to the transaction", buyer.Id, product.Id);

            var transactionManager = new TransactionManager(_context);

            // begin the transaction 
            await using var transaction = _context.Database.BeginTransaction();
            buyer.Capital -= summedValue;

            // Create a new transaction about
            // who bought
            // what time did the transaction take place
            // the amount the customer is buying
            var customerFullTransaction =
                await transactionManager.AddTransaction(new Transactions
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

                await UpdateProductInDisplay(product, transactionQuantity);

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


            _context.Transactions.Update(customerFullTransaction);
            _context.SaveChanges();
            await transaction.CommitAsync();
            //await CheckIfNeedReSupply(product.toListFromOne());
        }

        //private async Task CheckIfNeedReSupply(IQueryable<Products> products)
        //{
        //    var productsFromDepartments = CheckNeedForResupply(products);

        //    if (productsFromDepartments.Any())
        //        await Task.Run(() => productsFromDepartments
        //            .ForEach(async x => await Supply(x.Item1.SupplierKey, x.Item1, x.Item2)));
        //}

        public List<Tuple<Products, int>> CheckNeedForResupply(IQueryable<Products> products)
        {
            var productsFromDepartments =
                (from product in products
                 join minQuantity in _context.ProductMinQuantity
                     on product.Id equals minQuantity.ProductKey
                 where product.QuantityInStorage < minQuantity.MinStorage
                 select new
                 {
                     item1 = product,
                     QuantityToBeSupplied = minQuantity.MinStorage - product.QuantityInStorage
                 })
                 .AsEnumerable()
                 .Select(x => new Tuple<Products, int>(x.item1, x.QuantityToBeSupplied))
                 .ToList();

            return productsFromDepartments;
        }

        public async Task UpdateProductInDisplay(Products productBought, int transactionQuantity)
        {
            var departmentConnection = _context.Department.FirstOrDefault(x => x.Prod_Id == productBought.Id);

            if (departmentConnection == null)
                throw new ConnectionExceptions("Connection in department by product id was not found", productBought.Id);
            departmentConnection.Number -= transactionQuantity;

            //TODO Enum for operation - +
            productBought.QuantityInDisplay -= transactionQuantity;

            await _context.SaveChangesAsync();
        }

        public void CheckValidityOfBuy(BuyActionClass buyClass)
        {
            if (buyClass.CustomerKey == 0)
                throw new ValidityException("Please select a customer");
            if (buyClass.ProductKey == 0)
                throw new ValidityException("Please select a product");
            if (buyClass.Quantity == 0)
                throw new ValidityException("Please give an amount of product you want to buy");
        }

        public Task<List<Products>> BringAllProducts()
        {
            try
            {
                var products = _context.Products.Select(x => x).ToListAsync();

                return products;
            }
            catch (Exception err)
            {
                throw err;
            }

        }
    }
}
