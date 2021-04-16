using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Store_chain.DataLayer;
using Store_chain.Model;

namespace Store_chain.Data
{
    public class SupplierManager
    {
        private readonly StoreChainContext _context;
        public SupplierManager(StoreChainContext context)
        {
            _context = context;
        }

        public async Task<List<Suppliers>> BringSuppliers()
        {
           return await _context.Suppliers.ToListAsync();
        }

        public async Task<Suppliers> BringSupplier(int id)
        {
            return await _context.Suppliers.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Suppliers> FindSuppliersAsync(int id)
        {
            return await _context.Suppliers.FindAsync(id);
        }

        public bool AnySuppliers(int id)
        {
            return _context.Suppliers.Any(e => e.Id == id);
        }
        public async Task CreateSupplier(Suppliers supplier)
        {
            _context.Add(supplier);
            await _context.SaveChangesAsync();
        }

        public async Task EditSupplier(Suppliers supplier)
        {
            _context.Update(supplier);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCustomer(int id)
        {
            var suppliers = await _context.Suppliers.FindAsync(id);
            _context.Suppliers.Remove(suppliers);
            await _context.SaveChangesAsync();
        }
    }
}
