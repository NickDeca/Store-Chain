﻿using Store_chain.Model;
using Store_chain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store_chain.HelperMethods
{
    public class ActionsHelper
    {
        private readonly StoreChainContext _context;
        public ActionsHelper(StoreChainContext context)
        {
            _context = context;
        }

        public async Task Supply(Suppliers supplier, Products product, int productQuantity)
        {
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
            await using (var transaction = _context.Database.BeginTransaction())
            {
                var summedValue = cart.Sum(x => x.CostSold);
                if (summedValue == 0)
                    throw new Exception("No Products bought");
                // TODO Transaction Table 

                if (buyer.Capital - summedValue <= 0)
                    throw new Exception("Customer Does not have the capital required to the transaction");

                buyer.Capital -= summedValue;
                try
                {
                    _context.Customers.Update(buyer);
                    foreach (var product in cart)
                    {
                        var department = _context.StoreDepartments.FirstOrDefault(x => x.Id == product.Department && x.ProductKey == product.Id);
                        CheckIfNeedReSupply(department);
                        _context.StoreDepartments.Remove(department ?? throw new Exception($"Product with Id:{product.Id} does not exist in Department"));
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                store.Capital += summedValue;

                await transaction.CommitAsync();
            }

        }

        private void CheckIfNeedReSupply(StoreDepartments department)
        {
            throw new NotImplementedException();
        }
    }
}
