using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Store_chain.DataLayer;
using Store_chain.Models;

namespace Store_chain.Data
{
    public class ProductManager
    {
        private readonly StoreChainContext _context;
        public ProductManager(StoreChainContext context)
        {
            _context = context;
        }

        public async Task<List<Products>> BringProducts()
        {
            return await _context.Products.Select(x => x).ToListAsync();
        }

        public async Task<Products> BringProduct(int id)
        {
            return await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Products> FindProduct(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public bool AnyProducts(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        public async Task CreateProduct(Products product)
        {
            product.DepartmentForeignId = product.Department;
            _context.Add(product);
            _context.ProductMinQuantity.Add(new ProductMinQuantity { ProductKey = product.Id, MinDisplay = product.MaxDisplay, MinStorage = product.MinStorage });
            await _context.SaveChangesAsync();
        }

        public async Task EditProduct(Products product)
        {
            product.DepartmentForeignId = product.Department;
            _context.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProduct(int id)
        {
            var products = await _context.Products.FindAsync(id);
            _context.Products.Remove(products);
            await _context.SaveChangesAsync();
        }
    }
}
