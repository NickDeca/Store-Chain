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

            var transaction = new Transactions
            {
                RecipientKey = supplierKey,
                ProviderKey = 0, // 0 is the shop
                DateOfTransaction = timeOfTransaction,
                ErrorText = string.Empty,
                State = (int)StateEnum.UndeterminedState
            };

            try
            {
                var boughtValue = product.BoughtFromSuppliersCost * productQuantity;

                UpdateSuppliersDue(supplierKey, boughtValue);

                UpdateProductInStorage(product, supplierKey, productQuantity);

                transaction.Capital = boughtValue;
                await _context.SaveChangesAsync(); //TODO auto mporei na figei gt dn kanoume add transaction alla pio katw kanoume 

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

                throw;
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
        }

        /// <summary>
        /// Add products displayed in their respective department in the store
        /// </summary>
        /// <param name="product"></param>
        /// <param name="numToBeDisplayed"></param>
        /// <param name="department"></param>
        /// <returns></returns>
        public async Task Display(Products product, int numToBeDisplayed, int department)
        {
            //Check if product passed is in the database
            var toBeSavedProduct = _context.Products.Find(product);

            if (toBeSavedProduct == null)
                throw new Exception("No such Product in the database");

            // up the number of displayed 
            toBeSavedProduct.QuantityInDisplay += numToBeDisplayed;

            // check if connected with the department 
            var productAlreadyInDepartment = _context.Department
                .FirstOrDefault(x => x.Id == department && x.Prod_Id == product.Id);

            // if the connection does not exist, create it
            if (productAlreadyInDepartment == null)
            {
                _context.Department
                    .Add(new Department
                    {
                        DepartmentKey = department,
                        Description = product.Description,
                        Number = numToBeDisplayed,
                        State = product.MaxDisplay == numToBeDisplayed
                            ? (int)DepartmentProductState.Filled
                            : (int)DepartmentProductState.NeedFilling
                    });
            }
            // else update the number of products in display 
            else
            {
                productAlreadyInDepartment.Number += numToBeDisplayed;

                if (productAlreadyInDepartment.Number == product.MaxDisplay)
                    productAlreadyInDepartment.State = (int)DepartmentProductState.Filled;
                else if (productAlreadyInDepartment.Number > product.MaxDisplay)
                    productAlreadyInDepartment.State = (int)DepartmentProductState.OverFilled;
                else
                    productAlreadyInDepartment.State = (int)DepartmentProductState.NeedFilling;
            }

            await _context.SaveChangesAsync();
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
                try //TODO I can remove this 
                {
                    buyer.Capital -= summedValue;

                    // Create a new transaction about
                    // who bought
                    // what time did the transaction take place
                    // the amount the customer is buying
                    // set the transaction level to major and the errors to non
                    var customerFullTransaction =
                        transactionManager.AddTransaction(new Transactions
                        {
                            RecipientKey = buyer.Id,
                            ProductKey = product.Id,
                            DateOfTransaction = timeOfTransaction,
                            Capital = summedValue,
                            ErrorText = string.Empty
                        });

                    try
                    {
                        // update the customer with the new capital after paying
                        _context.Customers.Update(buyer);

                        await UpdateProductInDisplay(product);

                        // create another entry in the transactions, with all the info needed to be recognized
                        // subset of the major 
                        customerFullTransaction.ProductQuantity = product.TransactionQuantity;
                        customerFullTransaction.ErrorText = "OK!";
                        customerFullTransaction.State = (int) StateEnum.OkState;
                    }
                    catch (Exception error)
                    {
                        customerFullTransaction.State = (int)StateEnum.ErrorState;
                        customerFullTransaction.ErrorText = error.Message;
                        throw;
                    }
                    // _context.StoreDepartments.Remove(department ?? throw new Exception($"Product with Id:{product.Id} does not exist in Department"));

                    customerFullTransaction.State = (int)StateEnum.OkState;
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
                await CheckIfNeedReSupply(product.toListFromOne());

                _context.CentralStoreCapital.Add(new CentralStoreCapital { Capital = summedValue, TransactionKey = 0 });
                //store.Capital += summedValue;

                _context.SaveChanges();
                await transaction.CommitAsync();
            }
        }

        private async Task CheckIfNeedReSupply(List<Products> products)
        {
            var productsFromDepartments =
                (from product in products
                 join minQuantity in _context.MinQuantities
                     on product.Id equals minQuantity.id
                 where product.QuantityInStorage < minQuantity.MinStorage
                 select new
                 {
                     product,
                     QuantityToBeSupplied = minQuantity.MinStorage - product.QuantityInStorage
                 }).ToList();

            await Task.Run(() => productsFromDepartments
                .ForEach(async x => await Supply(x.product.SupplierKey, x.product, x.QuantityToBeSupplied)));
        }

        public async Task UpdateProductInDisplay(Products productBought)
        {
            var departmentConnection = _context.Department.FirstOrDefault(x => x.Prod_Id == productBought.Id);

            if(departmentConnection == null)
                throw new Exception();
            departmentConnection.Number -= productBought.TransactionQuantity;

            productBought.QuantityInDisplay -= productBought.TransactionQuantity;
            _context.SaveChanges();
        }

        public void CheckValidityOfBuy(BuyActionClass buyClass)
        {
            if (buyClass.CustomerKey == 0)
                throw new Exception("Please select a customer");
            if(buyClass.ProductKey == 0)
                throw new Exception("Please select a product");
            if(buyClass.Quantity == 0)
                throw new Exception("Please give an amount of product you want to buy");
        }

        public List<Products> BringAllProducts()
        {
            var products =
                (from productSolo in _context.Products
                 join department in _context.Department
                        on productSolo.Department equals department.Id
                    select new Products
                    {
                        Id = productSolo.Id,
                        department = department,
                        Department = productSolo.Department,
                        BoughtFromSuppliersCost = productSolo.BoughtFromSuppliersCost,
                        Category = productSolo.Category,
                        Description = productSolo.Description,
                        IsDisplay = productSolo.IsDisplay,
                        QuantityInDisplay = productSolo.QuantityInDisplay,
                        QuantityInStorage = productSolo.QuantityInStorage,
                        SoldToCustomersCost = productSolo.SoldToCustomersCost,
                        SupplierKey = productSolo.SupplierKey,
                        TransactionQuantity = productSolo.SupplierKey
                    }
                ).ToList();

            return products;
        }
    }
}
