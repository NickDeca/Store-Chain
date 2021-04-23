using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store_chain.Data.Managers;
using Store_chain.DataLayer;
using Store_chain.Model;

namespace Store_chain.Controllers
{
    public class SuppliersController : BaseController<Suppliers>
    {
        private readonly IManager<Suppliers> _manager;

        public SuppliersController(IManager<Suppliers> manager) : base(manager)
        {
            _manager = manager;
        }

        // POST: Suppliers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PaymentDue,Category,Description,Name")] Suppliers suppliers)
        {
            if (ModelState.IsValid)
            {
                await _manager.Create(suppliers);
                return RedirectToAction(nameof(Index));
            }
            return View(suppliers);
        }

        // POST: Suppliers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PaymentDue,Category,Description,Name")] Suppliers suppliers)
        {
            if (id != suppliers.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _manager.Edit(suppliers);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnyExists(suppliers.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(suppliers);
        }

        // POST: Suppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _manager.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
