using Store_chain.Model;
using Store_chain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Store_chain.Data;
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

        public async Task Supply(int supplierKey, Products product, int productQuantity)
        {
            var supplier = _context.Suppliers.Find(supplierKey);

            if (product.SupplierKey != supplier.Id)
                throw new Exception("The specified supplier does not contain the product");

            //TODO see better method to get from DB.
            var boughtValue = product.CostBought * productQuantity;
            var toBesSavedSupplier = _context.Suppliers.Find(supplier);
            var toBeSavedProduct = _context.Products.Find(product);

            if (toBeSavedProduct == null)
                throw new Exception("No such Product in the database");
            if (toBesSavedSupplier == null)
                throw new Exception("No such Supplier in the database");
            toBesSavedSupplier.PaymentDue += boughtValue;
            toBeSavedProduct.QuantityInStorage += productQuantity;

            _context.Suppliers.Update(toBesSavedSupplier);
            _context.Products.Update(toBeSavedProduct);

            await _context.SaveChangesAsync();
        }

        public async Task Display(Products product, int numToBeDisplayed)
        {
            var toBeSavedProduct = _context.Products.Find(product);

            if (toBeSavedProduct == null)
                throw new Exception("No such Product in the database");

            toBeSavedProduct.QuantityInDisplay += numToBeDisplayed;

            var storeDepartment = _context.StoreDepartments.FirstOrDefault(x => x.Id == product.Department);

            if (storeDepartment == null)
                throw new Exception("Department with Product's Department Id does not exist");
            //TODO update Department to hold num of products held there
            storeDepartment.MaxProducts += numToBeDisplayed;

            _context.StoreDepartments.Update(storeDepartment);
            await _context.SaveChangesAsync();
        }

        public async Task Buy(List<Products> cart, Customers buyer, Store store)
        {
            // TODO product quantity
            var summedValue = cart.Sum(x => x.CostSold * x.TransactionQuantity);
            if (summedValue == 0)
                throw new Exception("No Products bought");
            // TODO Transaction Table 

            if (buyer.Capital - summedValue <= 0)
                throw new Exception("Customer Does not have the capital required to the transaction");

            var transactionManager = new TransactionManager(_context);

            await using (var transaction = _context.Database.BeginTransaction())
            {
                buyer.Capital -= summedValue;
                try
                {
                    _context.Customers.Update(buyer);
                    foreach (var product in cart)
                    {
                        var department = _context.StoreDepartments.FirstOrDefault(x => x.Id == product.Department && x.ProductKey == product.Id);

                        try
                        {
                            transactionManager.AddTransaction(new Transactions
                            {
                                CustomerKey = buyer.Id,
                                ProductKey = product.Id,
                                DateOfTransaction = DateTime.Now,
                                Capital = product.CostSold,
                                ProductQuantity = product.TransactionQuantity
                            });

                            // Todo state 1 here
                        }
                        catch (Exception error)
                        {
                            // Todo State 2 here
                        }

                        _context.StoreDepartments.Remove(department ?? throw new Exception($"Product with Id:{product.Id} does not exist in Department"));
                    }
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
                await CheckIfNeedReSupply(cart);
                store.Capital += summedValue;

                _context.SaveChanges();
                await transaction.CommitAsync();
            }
        }

        private async Task CheckIfNeedReSupply(IEnumerable<Products> products)
        {
            var productsFromDepartments = (from product in products
                                           join minQuantity in _context.MinQuantities
                                               on product.Id equals minQuantity.id
                                           where product.QuantityInStorage < minQuantity.MinStorage
                                           select new
                                           {
                                               product,
                                               QuantityToBeSupplied = minQuantity.MinStorage - product.QuantityInStorage
                                           }).ToList();
            await Task.Run(() => productsFromDepartments.ForEach( async x => await Supply(x.product.SupplierKey, x.product, x.QuantityToBeSupplied)));
        }
    }
}
