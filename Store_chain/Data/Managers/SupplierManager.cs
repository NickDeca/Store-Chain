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
    public class SupplierManager : /*BaseManager<Suppliers>*/ IManager<Suppliers, SupplierViewDTO>
    {
        private readonly StoreChainContext _context;
        public SupplierManager(StoreChainContext context)
        {
            _context = context;
        }

        public async Task<List<Suppliers>> BringAll()
        {
            return await _context.Suppliers.ToListAsync();
        }

        public async Task<Suppliers> BringOne(int id)
        {
            return await _context.Suppliers.FirstOrDefaultAsync(m => m.Id == id);
        }
        public async Task<Suppliers> BringOneException(int id)
        {
            return await _context.Suppliers.FirstOrDefaultAsync(m => m.Id == id) ?? throw new DbNotFoundEntityException();
        }

        public async Task<Suppliers> FindOne(int id)
        {
            return await _context.Suppliers.FindAsync(id);
        }

        public Task<SupplierViewDTO> FindOneDTO(int value)
        {
            throw new NotImplementedException();
        }

        public bool Any(int id)
        {
            return _context.Suppliers.Any(e => e.Id == id);
        }
        public async Task Create(Suppliers supplier)
        {
            _context.Add(supplier);
            await _context.SaveChangesAsync();
        }

        public async Task Edit(int id, dynamic DTO)
        {
            var supplier = await BringOneException(id);
            try
            {
                ChangeDTOToFull(ref supplier, DTO);
                _context.Update(supplier);
                await _context.SaveChangesAsync();
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        private void ChangeDTOToFull(ref Suppliers fullClass, dynamic DTO)
        {
            if (DTO.PaymentDue != null && DTO.PaymentDue != default(decimal))
                fullClass.PaymentDue = DTO.PaymentDue;
            if (!string.IsNullOrEmpty(DTO.Description))
                fullClass.Description = DTO.Description;
            if (DTO.Category != null && DTO.Category != default(decimal))
                fullClass.Category = DTO.Category;
            if (!string.IsNullOrEmpty(DTO.Name))
                fullClass.Name = DTO.Name;
        }

        public async Task Delete(int id)
        {
            var suppliers = await _context.Suppliers.FindAsync(id);
            _context.Suppliers.Remove(suppliers);
            await _context.SaveChangesAsync();
        }
    }
}
