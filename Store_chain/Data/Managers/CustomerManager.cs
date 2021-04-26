using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Store_chain.Data.DTO;
using Store_chain.DataLayer;
using Store_chain.Exceptions;
using Store_chain.Model;

namespace Store_chain.Data.Managers
{
    public class CustomerManager : IManager<Customers, CustomerEditViewDTO>
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

        public async Task<CustomerEditViewDTO> FindOneDTO(int id)
        {
            var concrete = await _context.Customers.FindAsync(id);
            return new CustomerEditViewDTO
            {
                Description = concrete.Description,
                Capital= concrete.Capital,
                FirstName = concrete.FirstName,
                LastName = concrete.LastName
            };
        }

        private Task<dynamic> CustomerEditViewDTO()
        {
            throw new NotImplementedException();
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

        public async Task Edit(int id, dynamic DTO)
        {
            var customer = await TryBringOne(id);
            try
            {
                ChangeDTOToFull(ref customer, DTO);
                _context.Update(customer);
                await _context.SaveChangesAsync();
            }
            catch (Exception err)
            {
                throw err;
            }

        }

        public async Task Delete(int id)
        {
            var customers = await _context.Customers.FindAsync(id);
            _context.Customers.Remove(customers);
            await _context.SaveChangesAsync();
        }

        public void ChangeDTOToFull(ref Customers fullClass, CustomerEditViewDTO DTO)
        {
            if (!string.IsNullOrEmpty(DTO.Description) && !CheckIfSame(fullClass.Description, DTO.Description))
                fullClass.Description = DTO.Description;

            if (DTO.Capital != null && DTO.Capital != default(decimal?) && !CheckIfSame(fullClass.Capital, DTO.Capital))
                fullClass.Capital = DTO.Capital;

            if (!string.IsNullOrEmpty(DTO.FirstName) && !CheckIfSame(fullClass.FirstName, DTO.FirstName))
                fullClass.FirstName = DTO.FirstName;

            if (!string.IsNullOrEmpty(DTO.LastName) && !CheckIfSame(fullClass.LastName, DTO.LastName))
                fullClass.LastName = DTO.LastName;
        }

        private bool CheckIfSame<T,TDTO>(T fullMember, TDTO DTOMember)
        {
            if (!typeof(T).Equals(typeof(TDTO)))
                throw new Exception($"The type of {typeof(T)} of full class cannot be cast to {typeof(TDTO)} of DTO class");
            return fullMember.Equals(DTOMember);
        }
    }
}
