using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Store_chain.Enums;
using Store_chain.Model;
using Store_chain.Models;

namespace Store_chain.Controllers
{
    public class ProductDepartmentController : Controller
    {
        private StoreChainContext _context;
        public ProductDepartmentController(StoreChainContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? id)
        {
            var products = _context.Products.Where(x => x.Department == id)
                .ToList();

            var join = (from product in products
                        join department in _context.ProductDepartments
                            on product.Id equals department.ProductKey
                        select new ProductDepartmentDto
                        {
                            Product = product,
                            NumberDisplayed = department.Number,
                            State = ((StateEnum)department.State).ToString()
                        }).ToList();

            ViewBag["Department"] = _context.StoreDepartments.FirstOrDefault(x => x.Id == id)?.Description ?? string.Empty;

            return View(join);
        }

        [HttpPost]
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }


    public class ProductDepartmentDto
    {
        public Products Product { get; set; }
        public int? NumberDisplayed { get; set; }
        public string State { get; set; }

    }
}