using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Store_chain.DataLayer;
using Store_chain.Exceptions;
using Store_chain.Model;

namespace Store_chain.Data.Managers
{
    public class CustomerManager : IManager<Customers>
    {
        private readonly StoreChainContext _context;
        public CustomerManager(StoreChainContext context)
        {
            _context = context;
        }

        public async Task<List<Customers>> BringAll()
        {
            return await _context.Customers.ToListAsync();
        }

        public async Task<Customers> BringOne(int id)
        {
            return await _context.Customers.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Customers> TryBringOne(int id)
        {
            return await _context.Customers.FirstOrDefaultAsync(m => m.Id == id) ?? throw new DbNotFoundEntityException();
        }

        public async Task<Customers> FindOne(int id)
        {
            return await _context.Customers.FindAsync(id);
        }

        public bool Any(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }

        public async Task Create(Customers customer)
        {
            _context.Add(customer);
            await _context.SaveChangesAsync();
        }

        public async Task Edit(Customers customer)
        {
            _context.Update(customer);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var customers = await _context.Customers.FindAsync(id);
            _context.Customers.Remove(customers);
            await _context.SaveChangesAsync();
        }

        public void ChangeDTOToFull(ref dynamic fullClass, dynamic DTO)
        {
            throw new NotImplementedException();
        }
    }
}
