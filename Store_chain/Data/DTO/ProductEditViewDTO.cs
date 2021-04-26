using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store_chain.Data.DTO
{
    public class ProductEditViewDTO
    {
        public int SupplierKey { get; set; }
        public int Category { get; set; }
        public int Department { get; set; }
        public decimal SoldToCustomersCost { get; set; }
        public decimal BoughtFromSuppliersCost { get; set; }
        public int TransactionQuantity { get; set; }
        public int QuantityInStorage { get; set; }
        public int QuantityInDisplay { get; set; }
        public int MaxDisplay { get; set; }
        public int MinStorage { get; set; }
    }
}
