using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Store_chain.DataLayer;
using Store_chain.Model;
using Store_chain.Models;
using Store_chain.Exceptions;
using Store_chain.Data.DTO;

namespace Store_chain.Data.Managers
{
    public class ProductManager : IManager<Products, ProductEditViewDTO>
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

        public async Task<Products> TryBringOne(int id)
        {
            return await _context.Products.FirstOrDefaultAsync(m => m.Id == id) ?? throw new DbNotFoundEntityException();
        }

        public async Task<Products> FindOne(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public Task<ProductEditViewDTO> FindOneDTO(int value)
        {
            throw new NotImplementedException();
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

        public async Task Edit(int id, dynamic DTO)
        {
            var product = await TryBringOne(id);
            try
            {
                ChangeDTOToFull(ref product, DTO);
                product.DepartmentForeignId = product.Department;
                _context.Update(product);
                await _context.SaveChangesAsync();
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public async Task Delete(int id)
        {
            var products = await _context.Products.FindAsync(id);
            _context.Products.Remove(products);
            await _context.SaveChangesAsync();
        }

        public void ChangeDTOToFull(ref Products fullClass, dynamic DTO)
        {
            if (DTO.SupplierKey != default(int))
                fullClass.SupplierKey = DTO.SupplierKey;
            if (DTO.Category != default(int))
                fullClass.Category = DTO.Category;

            if (DTO.Department != default(int))
                fullClass.Department = DTO.Department;

            if (DTO.SoldToCustomersCost != default(decimal))
                fullClass.SoldToCustomersCost = DTO.SoldToCustomersCost;

            if (DTO.BoughtFromSuppliersCost != default(decimal))
                fullClass.SupplierKey = DTO.SupplierKey;

            if (DTO.TransactionQuantity != default(int))
                fullClass.TransactionQuantity = DTO.TransactionQuantity;

            if (DTO.QuantityInStorage != default(int))
                fullClass.QuantityInStorage = DTO.QuantityInStorage;

            if (DTO.QuantityInDisplay != default(int))
                fullClass.QuantityInDisplay = DTO.QuantityInDisplay;

            if (DTO.MaxDisplay != default(int))
                fullClass.MaxDisplay = DTO.MaxDisplay;

            if (DTO.MinStorage != default(int))
                fullClass.MinStorage = DTO.MinStorage;
        }
    }
}
