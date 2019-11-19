using System;
using System.Linq;
using System.Threading.Tasks;
using Store_chain.Model;
using Store_chain.Models;

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

            if(toBeSavedProduct == null)
                throw new Exception("No such Product in the database");

            toBeSavedProduct.QuantityInDisplay += numToBeDisplayed;

            var storeDepartment = _context.StoreDepartments.FirstOrDefault(x => x.Id == product.Department);

            if(storeDepartment == null)
                throw new Exception("Department with Product's Deparment Id does not exist");
            //TODO update Department to hold num of products held there
            storeDepartment.MaxProducts += numToBeDisplayed;

            _context.StoreDepartments.Update(storeDepartment);
            await _context.SaveChangesAsync();
        }
    }
}
