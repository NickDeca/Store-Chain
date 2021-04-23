using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Store_chain.DataLayer;
using Store_chain.Model;
using Store_chain.Models;

namespace Store_chain.Data.Managers
{
    public class ProductManager : IManager<Products>
    {
        private readonly StoreChainContext _context;
        public ProductManager(StoreChainContext context)
        {
            _context = context;
        }

        public async Task<List<Products>> BringAll()
        {
            return await _context.Products.Select(x => x).ToListAsync();
        }

        public async Task<Products> BringOne(int id)
        {
            return await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Products> FindOne(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public bool Any(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        public async Task Create(Products product)
        {
            product.DepartmentForeignId = product.Department;
            _context.Add(product);
            _context.ProductMinQuantity.Add(new ProductMinQuantity { ProductKey = product.Id, MinDisplay = product.MaxDisplay, MinStorage = product.MinStorage });
            await _context.SaveChangesAsync();
        }

        public async Task Edit(Products product)
        {
            product.DepartmentForeignId = product.Department;
            _context.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var products = await _context.Products.FindAsync(id);
            _context.Products.Remove(products);
            await _context.SaveChangesAsync();
        }
    }
}
