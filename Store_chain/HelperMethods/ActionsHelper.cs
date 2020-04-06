using Store_chain.Model;
using Store_chain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Store_chain.Data;
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

        public async Task Supply(int supplierKey, Products product, int productQuantity)
        {
            try
            {
                var supplier = _context.Suppliers.FirstOrDefault(x => x.Id == supplierKey);

                if(supplier == null)
                    throw new Exception("The specified supplier was not found");

                if (product.SupplierKey != supplier.Id)
                    throw new Exception("The specified supplier does not contain the product");

                var boughtValue = product.CostBought * productQuantity;
                
                supplier.PaymentDue += boughtValue;
                product.QuantityInStorage += productQuantity;
                //TODO subtract money paid from store
                //TODO transaction table enhmerwsh

                _context.Suppliers.Update(supplier);
                _context.Products.Update(product);

                await _context.SaveChangesAsync();
            }
            catch (Exception error)
            {
                throw error;
            }
        }

        public async Task Display(Products product, int numToBeDisplayed, int department)
        {
            var toBeSavedProduct = _context.Products.Find(product);

            if (toBeSavedProduct == null)
                throw new Exception("No such Product in the database");

            toBeSavedProduct.QuantityInDisplay += numToBeDisplayed;

            var storeDepartment = _context.StoreDepartments.FirstOrDefault(x => x.Id == product.Department);

            if (storeDepartment == null)
                throw new Exception("Department with Product's Department Id does not exist");

            var productDepartment = _context.ProductDepartments.FirstOrDefault(x => x.DepartmentKey == department
                                                                          && x.ProductKey == product.Id);

            if (productDepartment == null)
                _context.ProductDepartments.Add(new ProductCategories
                {
                    DepartmentKey = department,
                    Description = product.Description,
                    Number = numToBeDisplayed,
                    State = product.QuantityInDisplay == numToBeDisplayed
                        ? (int)DepartmentProductState.Filled
                        : (int)DepartmentProductState.NeedFilling
                });
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

            storeDepartment.MaxProducts += numToBeDisplayed;

            _context.StoreDepartments.Update(storeDepartment);
            await _context.SaveChangesAsync();
        }

        public async Task Buy(List<Products> cart, Customers buyer)
        {
            // TODO product quantity
            var summedValue = cart.Sum(x => x.CostSold * x.TransactionQuantity);
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
                            CustomerKey = buyer.Id,
                            DateOfTransaction = timeOfTransaction,
                            Capital = summedValue,
                            Major = (int)isMajorTransaction.Major,
                            ErrorText = string.Empty
                        });
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
                                DateOfTransaction = timeOfTransaction,
                                Capital = product.CostSold,
                                ProductQuantity = product.TransactionQuantity,
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

                        _context.StoreDepartments.Remove(department ?? throw new Exception($"Product with Id:{product.Id} does not exist in Department"));
                    }
                    customerFullTransaction.State = (int)StateEnum.OkState;
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
                await CheckIfNeedReSupply(cart);

                _context.Store.Add(new Store() {Capital = summedValue , TimeOfTransaction = timeOfTransaction , TransactionKey = 0 });
                //store.Capital += summedValue;

                _context.SaveChanges();
                await transaction.CommitAsync();
            }
        }

        private async Task CheckIfNeedReSupply(IEnumerable<Products> products)
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
                                 .ForEach(async x => 
                                                await Supply(x.product.SupplierKey, x.product, x.QuantityToBeSupplied)));
        }
    }
}
