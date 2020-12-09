using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Store_chain.Data;

namespace Store_chain.Models
{
    public partial class Products
    {
        [Key]
        public int Id { get; set; }
        [Column("Supplier_Key")]
        [DisplayName("Supplier")]
        public int SupplierKey { get; set; }
        public int Category { get; set; }
        public int Department { get; set; }
        public string Description { get; set; }
        [DisplayName("In Display")]
        public bool IsDisplay { get; set; }

        [Column("CostSold")]
        [DisplayName("Cost Sold")]
        public decimal SoldToCustomersCost { get; set; }
        [Column("CostBought")]
        [DisplayName("Cost Bought")]
        public decimal BoughtFromSuppliersCost { get; set; }
        [DisplayName("Quantity")]
        public int TransactionQuantity { get; set; }
        [DisplayName("Storage Quantity")]
        public int QuantityInStorage { get; set; }
        [DisplayName("Display Quantity")]
        public int QuantityInDisplay { get; set; }

        [DisplayName("Maximum Display Quantity")]
        public int MaxDisplay { get; set; }
        [DisplayName("Minimum Storage Quantity")]
        public int MinStorage { get; set; }

        public Department department { get;set; }
    }
}
