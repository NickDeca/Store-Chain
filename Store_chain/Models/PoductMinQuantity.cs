using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store_chain.Model
{
    public class ProductMinQuantity : BaseModel
    {
        public int ProductKey { get; set; }
        public int MinDisplay { get; set; }
        public int MinStorage { get; set; }
    }
}
