using System.ComponentModel.DataAnnotations;

namespace Store_chain.Models
{
    public class ProductMinQuantity
    {
        [Key]
        public int id { get; set; }
        public int ProductKey { get; set; }
        public int MinDisplay { get; set; }
        public int MinStorage { get; set; }
    }
}
