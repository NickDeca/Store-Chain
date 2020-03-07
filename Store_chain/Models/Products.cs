using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store_chain.Models
{
    public partial class Products
    {
        [Key]
        public int Id { get; set; }
        [Column("Supplier_Key")]
        public int SupplierKey { get; set; }
        public int Category { get; set; }
        public int Department { get; set; }
        public string Description { get; set; }
        public bool IsDisplay { get; set; }
        public decimal CostSold { get; set; }
        public decimal CostBought { get; set; }
        public int TransactionQuantity { get; set; }
        public int QuantityInStorage { get; set; }

        public int QuantityInDisplay { get; set; }
    }
}
