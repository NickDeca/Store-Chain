using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Store_chain.DataLayer;

namespace Store_chain.Controllers
{
    public class StoreController : Controller
    {
        private readonly StoreChainContext _context;

        public StoreController(StoreChainContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var model = _context.CentralStoreCapital.Any() ? _context.CentralStoreCapital.Select(x => x).LastOrDefault() : null;
            return View(model);
        }
    }
}
