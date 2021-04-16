using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Store_chain.DataLayer;
using Store_chain.Model;

namespace Store_chain.Data
{
    public class CustomerManager
    {
        private readonly StoreChainContext _context;
        public CustomerManager(StoreChainContext context)
        {
            _context = context;
        }

        public async Task<List<Customers>> BringCustomers()
        {
            return await _context.Customers.ToListAsync();
        }

        public async Task<Customers> BringCustomer(int id)
        {
            return await _context.Customers.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Customers> FindCustomer(int id)
        {
            return await _context.Customers.FindAsync(id);
        }

        public bool AnyCustomer(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }

        public async Task CreateCustomer(Customers customer)
        {
            _context.Add(customer);
            await _context.SaveChangesAsync();
        }

        public async Task EditCustomer(Customers customer)
        {
            _context.Update(customer);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCustomer(int id)
        {
            var customers = await _context.Customers.FindAsync(id);
            _context.Customers.Remove(customers);
            await _context.SaveChangesAsync();
        }
    }
}
