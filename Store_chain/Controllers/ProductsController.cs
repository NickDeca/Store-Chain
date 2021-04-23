using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store_chain.Data.Managers;
using Store_chain.DataLayer;
using Store_chain.Models;

namespace Store_chain.Controllers
{
    public class ProductsController : BaseController<Products>
    {
        private readonly IManager<Products> _manager;

        public ProductsController(IManager<Products> manager) : base(manager)
        {
            _manager = manager;
        }
        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SupplierKey,Category,Department,Description,IsDisplay,CostSold,CostBought,TransactionQuantity,QuantityInStorage,QuantityInDisplay,MaxDisplay,MinStorage,SoldToCustomersCostAsString,BoughtFromSuppliersCost")] Products products)
        {
            if (ModelState.IsValid)
            {
                await _manager.Create(products);

                return RedirectToAction(nameof(Index));
            }
            return View(products);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SupplierKey,Category,Department,Description,CostSold,CostBought,TransactionQuantity,QuantityInStorage,QuantityInDisplay,MaxDisplay,MinStorage,SoldToCustomersCostAsString,BoughtFromSuppliersCost")] Products products)
        {
            if (id != products.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _manager.Edit(products);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnyExists(products.Id))
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
            return View(products);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _manager.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
