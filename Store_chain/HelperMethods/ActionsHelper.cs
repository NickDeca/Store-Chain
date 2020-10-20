﻿using Store_chain.Model;
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
                Major = (int)isMajorTransaction.Major,
                ErrorText = string.Empty,
                State = (int)StateEnum.UndeterminedState
            };

            try
            {
                var boughtValue = product.BoughtFromSuppliersCost * productQuantity;

                UpdateSuppliersDue(supplierKey, boughtValue);

                UpdateProductInStorage(product, supplierKey, productQuantity);

                transaction.Capital = boughtValue;
                await _context.SaveChangesAsync();
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
        /// 
        /// </summary>
        /// <param name="product"></param>
        /// <param name="numToBeDisplayed"></param>
        /// <param name="department"></param>
        /// <returns></returns>
        public async Task Display(Products product, int numToBeDisplayed, int department)
        {
            var toBeSavedProduct = _context.Products.Find(product);

            if (toBeSavedProduct == null)
                throw new Exception("No such Product in the database");

            toBeSavedProduct.QuantityInDisplay += numToBeDisplayed;
            
            var productDepartment = _context.Departments
                .FirstOrDefault(x => x.Id == department &&  x.Products.Any(z => z.Id == product.Id));

            // if the connection does not exist create it
            if (productDepartment == null)
            {
                _context.Departments
                    .Add(new Department
                    {
                        Id = department,
                        Description = product.Description,
                        Number = numToBeDisplayed,
                        State = product.QuantityInDisplay == numToBeDisplayed
                            ? (int)DepartmentProductState.Filled
                            : (int)DepartmentProductState.NeedFilling
                    });
            } 
            // else update the number of products in display 
            else
            {
                productDepartment.Number += numToBeDisplayed;

                if (productDepartment.Number == product.QuantityInDisplay)
                    productDepartment.State = (int)DepartmentProductState.Filled;
                else if (productDepartment.Number > product.QuantityInDisplay)
                    productDepartment.State = (int)DepartmentProductState.OverFilled;
                else
                    productDepartment.State = (int)DepartmentProductState.NeedFilling;
            }

            await _context.SaveChangesAsync();
        }

        public async Task Buy(Products cart, Customers buyer)
        {
            // TODO one product rewrite

            var summedValue = cart.SoldToCustomersCost * cart.TransactionQuantity;

            //var summedValue = cart.Sum(x => x.SoldToCustomersCost * x.TransactionQuantity);
            if (summedValue == 0)
                throw new Exception("No Products bought");
            var timeOfTransaction = DateTime.Now;

            if (buyer.Capital - summedValue <= 0)
                throw new Exception("Customer Does not have the capital required to the transaction");

            var transactionManager = new TransactionManager(_context);

            await using (var transaction = _context.Database.BeginTransaction())
            {
                buyer.Capital -= summedValue;
                try
                {
                    var customerFullTransaction =
                        transactionManager.AddTransaction(new Transactions
                        {
                            RecipientKey = buyer.Id,
                            DateOfTransaction = timeOfTransaction,
                            Capital = summedValue,
                            Major = (int)isMajorTransaction.Major,
                            ErrorText = string.Empty
                        });
                    _context.Customers.Update(buyer);
                        //var department = _context.StoreDepartments.FirstOrDefault(x => x.Id == product.Department && x.ProductKey == product.Id);
                        try
                        {
                            transactionManager.AddTransaction(new Transactions
                            {
                                RecipientKey = buyer.Id,
                                ProductKey = cart.Id,
                                DateOfTransaction = timeOfTransaction,
                                Capital = cart.SoldToCustomersCost,
                                ProductQuantity = cart.TransactionQuantity,
                                Major = (int)isMajorTransaction.Subset,
                                ErrorText = "OK!"
                            });
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
                await CheckIfNeedReSupply(cart);

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
    }
}
