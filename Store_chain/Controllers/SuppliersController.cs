using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store_chain.Data;
using Store_chain.DataLayer;
using Store_chain.Model;

namespace Store_chain.Controllers
{
    public class SuppliersController : Controller
    {
        private readonly SupplierManager _manager;

        public SuppliersController(StoreChainContext context)
        {
            _manager = new SupplierManager(context);
        }

        // GET: Suppliers
        public async Task<IActionResult> Index()
        {
            return View(await _manager.BringSuppliers());
        }

        // GET: Suppliers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var suppliers = _manager.BringSupplier(id.Value);
            if (suppliers == null)
            {
                return NotFound();
            }

            return View(suppliers);
        }

        // GET: Suppliers/Create
        public IActionResult Create()
        {
            return View();
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
                await _manager.CreateSupplier(suppliers);
                return RedirectToAction(nameof(Index));
            }
            return View(suppliers);
        }

        // GET: Suppliers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var suppliers = _manager.FindSuppliersAsync(id.Value);
            if (suppliers == null)
            {
                return NotFound();
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
                    _manager.EditSupplier(suppliers);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SuppliersExists(suppliers.Id))
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

        // GET: Suppliers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var suppliers = await _manager.BringSupplier(id.Value);
            if (suppliers == null)
            {
                return NotFound();
            }

            return View(suppliers);
        }

        // POST: Suppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _manager.DeleteCustomer(id);
            return RedirectToAction(nameof(Index));
        }

        private bool SuppliersExists(int id)
        {
            return _manager.AnySuppliers(id);
        }
    }
}
