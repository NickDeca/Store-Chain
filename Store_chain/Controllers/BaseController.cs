﻿namespace Store_chain.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Store_chain.Data.Managers;
    using Store_chain.DataLayer;
    using Store_chain.HelperMethods;
    using Store_chain.Models;

    public abstract class BaseController<TEntity> : Controller, IBaseController<TEntity> where TEntity : class, IBaseModel
    {
        //private readonly StoreChainContext _context;
        //private IActionsHelper _helper;
        private IManager<TEntity> _manager;

        public BaseController(IManager<TEntity> manager)//, StoreChainContext context, IActionsHelper helper)
        {
            //_context = context;
            //_helper = helper;
            _manager = manager;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _manager.BringAll());
        }

        public IActionResult Create()
        {
            return View();
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detailEntity = await _manager.BringOne(id.Value);
            if (detailEntity == null)
            {
                return NotFound();
            }

            return View(detailEntity);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _manager.FindOne(id.Value);
            if (products == null)
            {
                return NotFound();
            }
            return View(products);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _manager.BringOne(id.Value);
            if (products == null)
            {
                return NotFound();
            }

            return View(products);
        }

        public bool AnyExists(int id)
        {
            return _manager.Any(id);
        }
    }

    internal interface IBaseController<TEntity> where TEntity : class, IBaseModel
    {
        Task<IActionResult> Details(int? id);
    }
}
