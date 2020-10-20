using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Store_chain.Data;
using Store_chain.DataLayer;
using Store_chain.HelperMethods;

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
        public async Task<IActionResult> SupplyAction([Bind("SupplierKey,Id,QuantityInStorage")] int supplierKey, int id, int quantityInStorage)
        {
            try
            {
                var productForSupply = _context.Products.FirstOrDefault(x => x.Id == id);

                if(productForSupply == null)
                    throw new Exception("Product was not found!");

                await _helper.Supply(supplierKey, productForSupply, quantityInStorage);

            }
            catch (Exception error)
            {
                ModelState.AddModelError("SupplierKey", $"{Environment.NewLine}{error.Message}");
            }

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
        public async Task<IActionResult> BuyAction()
        {
            var products =  _context.Products.ToList();
            var buyClass = new BuyActionClass();
            
            return View((products,buyClass));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BuyAction([Bind("BuyClass")] 
             BuyActionClass buyClass)
        {
            //var customerKey = 0;
            var productForDisplay = _context.Products.FirstOrDefault(x => buyClass.ProductKey == x.Id);

            var products = _context.Products.ToList();
            var customer = _context.Customers.Find(buyClass.CustomerKey);

            var buyClassView = new BuyActionClass{ProductKey = buyClass.ProductKey,CustomerKey = buyClass.CustomerKey};

            await _helper.Buy(productForDisplay,  customer);
            return View((products,buyClassView));
        }
    }
}