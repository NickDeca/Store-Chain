using Store_chain.Model;
using Store_chain.Models;

namespace Store_chain.HelperMethods
{
    public class ActionsHelper
    {
        private readonly StoreChainContext _context;
        public ActionsHelper(StoreChainContext context)
        {
            _context = context;
        }

        public async void Supply(Suppliers supplier, Products product, int productQuantity)
        {

            var boughtValue = product.CostBought * productQuantity;
            var toBesSavedSupplier = _context.Suppliers.Find(supplier);
            var toBeSavedProduct = _context.Products.Find(product);

            toBesSavedSupplier.PaymentDue += boughtValue;
            toBeSavedProduct.QuantityInStorage += productQuantity;

            _context.Suppliers.Update(toBesSavedSupplier);
            _context.Products.Update(toBeSavedProduct);

            await _context.SaveChangesAsync();
        }
    }
}
