using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Store_chain.Model;
using Store_chain.Models;

namespace Store_chain.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private StoreChainContext _context;

        public HomeController(ILogger<HomeController> logger, StoreChainContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public void Supply(Suppliers supplier, Products product, int productQuantity)
        {
            var boughtValue = product.CostBought * productQuantity;
            var toBesSavedSupplier = _context.Suppliers.Find(supplier);
            var toBeSavedProduct = _context.Products.Find(product);

            toBesSavedSupplier.PaymentDue += boughtValue;
            toBeSavedProduct.QuantityInStorage += productQuantity;

            _context.Suppliers.Update(toBesSavedSupplier);
            _context.Products.Update(toBeSavedProduct);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
