using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store_chain.Models
{
    public class ProductMinQuantity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int ProductKey { get; set; }
        public int MinDisplay { get; set; }
        public int MinStorage { get; set; }
    }
}
