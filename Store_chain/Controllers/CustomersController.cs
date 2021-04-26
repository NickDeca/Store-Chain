using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store_chain.Data.DTO;
using Store_chain.Data.Managers;
using Store_chain.DataLayer;
using Store_chain.Exceptions;
using Store_chain.HelperMethods;
using Store_chain.Model;

namespace Store_chain.Controllers
{
    public class CustomersController : BaseController<Customers, CustomerEditViewDTO>
    {
        private readonly IManager<Customers, CustomerEditViewDTO> _manager;

        public CustomersController(IManager<Customers, CustomerEditViewDTO> manager) : base(manager)
        {
            _manager = manager;
        }

        // POST: Customers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Capital,Description,FirstName,LastName")] Customers customers)
        {
            if (ModelState.IsValid)
            {
                await _manager.Create(customers);
                return RedirectToAction(nameof(Index));
            }
            return View(customers);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CustomerEditViewDTO customerDTO)//[Bind("Id,Capital,Description,FirstName,LastName")] Customers customers)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _manager.Edit(id, customerDTO);
                }
                catch (DbNotFoundEntityException)
                {
                    return NotFound();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(customerDTO);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _manager.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
