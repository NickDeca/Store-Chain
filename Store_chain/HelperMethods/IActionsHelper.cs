using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Store_chain.Data;
using Store_chain.Model;
using Store_chain.Models;

namespace Store_chain.HelperMethods
{
    public interface IActionsHelper
    {
        Task<List<Products>> BringAllProducts();
        Task Buy(Products product, Customers buyer, int transactionQuantity);
        List<Tuple<Products, int>> CheckNeedForResupply(IQueryable<Products> products);
        void CheckValidityOfBuy(BuyActionClass buyClass);
        Task Display(int productKey, int numToBeDisplayed, int department);
        Task Supply(int supplierKey, Products product, int productQuantity);
        Task UpdateProductInDisplay(Products productBought, int transactionQuantity);
        void UpdateProductInStorage(Products product, int supplierKey, int productQuantity);
        void UpdateSuppliersDue(int supplierKey, decimal boughtValue);
    }
}