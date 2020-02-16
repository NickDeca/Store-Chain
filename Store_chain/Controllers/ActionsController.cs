using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Store_chain.HelperMethods;
using Store_chain.Model;

namespace Store_chain.Controllers
{
    public class ActionsController : Controller
    {
        private readonly StoreChainContext _context;
        private ActionsHelper _helper;

        public ActionsController(StoreChainContext context)
        {
            _context = context;
            _helper = new ActionsHelper(_context);
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        /// <summary>
        /// Get
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> SupplyAction(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products.FindAsync(id);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SupplyAction([Bind("SupplierKey,ProductKey,Quantity")] int supplierKey, int productKey, int quantity)
        {
            var productForSupply = _context.Products.FirstOrDefault(x => x.Id == productKey);

            await _helper.Supply(supplierKey, productForSupply, quantity);
            return View();
        }

        /// <summary>
        /// Get
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> DisplayAction(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products.FindAsync(id);
            return View(product);
        }

        /// <summary>
        /// Post
        /// </summary>
        /// <param name="productKey"></param>
        /// <param name="numToDisplay"></param>
        /// <param name="department"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DisplayAction([Bind("ProductKey,NumToDisplay,Department")] int productKey, int numToDisplay, int department)
        {

            var productForDisplay = _context.Products.FirstOrDefault(x => x.Id == productKey);
            await _helper.Display(productForDisplay, numToDisplay, department);
            return View();
        }

        /// <summary>
        /// Get
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> BuyAction(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products.FindAsync(id);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BuyAction([Bind("ProductKey,NumToDisplay,Department")] List<int> productKeys, int customerKey, int department)
        {
            var productForDisplay = _context.Products.Where(x => productKeys.Contains(x.Id));
            var customer = _context.Customers.Find(customerKey);

            await _helper.Buy(productForDisplay.ToList(),  customer);
            return View(productForDisplay.ToList());
        }
    }
}