using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store_chain.Data;
using Store_chain.DataLayer;
using Store_chain.Models;

namespace Store_chain.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ProductManager _manager;

        public ProductsController(StoreChainContext context)
        {
            _manager = new ProductManager(context);
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var products = _manager.BringProducts();
            
            return View(products);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = _manager.BringProduct(id.Value);
            if (products == null)
            {
                return NotFound();
            }

            return View(products);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
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
                await _manager.CreateProduct(products);

                return RedirectToAction(nameof(Index));
            }
            return View(products);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = _manager.FindProduct(id.Value);
            if (products == null)
            {
                return NotFound();
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
                    await _manager.EditProduct(products);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductsExists(products.Id))
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

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = _manager.BringProduct(id.Value);
            if (products == null)
            {
                return NotFound();
            }

            return View(products);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _manager.DeleteProduct(id);
            return RedirectToAction(nameof(Index));
        }

        private bool ProductsExists(int id)
        {
            return _manager.AnyProducts(id);
        }
    }
}
