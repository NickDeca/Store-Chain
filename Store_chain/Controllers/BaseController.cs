namespace Store_chain.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Store_chain.DataLayer;
    using Store_chain.HelperMethods;

    public abstract class BaseController : Controller
    {
        private readonly StoreChainContext _context;
        private IActionsHelper _helper;

        public BaseController(StoreChainContext context, IActionsHelper helper)
        {
            _context = context;
            _helper = helper;
        }


    }
}
