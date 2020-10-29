﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
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

                if (productForSupply == null)
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
            var products = _context.Products.ToList();

            return View(products);
        }

        /// <summary>
        /// Main Action for the Buy feature
        /// </summary>
        /// <param name="buyClass"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BuyAction(BuyActionClass buyClass)
        {
            // the products need to be outside the try block because they are used to reset the page 
            // in failure or success
            var products = _context.Products.ToList();
            try
            {
                // check if all the data send from the user are present
                _helper.CheckValidityOfBuy(buyClass);

                var productBought = _context.Products.FirstOrDefault(x => buyClass.ProductKey == x.Id);

                if (productBought == null)
                    throw new Exception("Major failure in server database");

                if (productBought.QuantityInDisplay < buyClass.Quantity)
                    throw new Exception("Cannot buy that amount of product");

                productBought.TransactionQuantity = buyClass.Quantity;

                var customer = await _context.Customers.FindAsync(buyClass.CustomerKey);

                if (customer == null)
                    throw new Exception("Customer not found retry!");

                await _helper.UpdateProductInDisplay(productBought);
                await _helper.Buy(productBought, customer);
                return View(products);
            }
            catch (Exception err)
            {
                ViewBag.AlertMessage = err.Message;
                return View(products);
            }
        }
    }
}