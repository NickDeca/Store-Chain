﻿namespace Store_chain.Models
{
    public partial class Products
    {
        public int Id { get; set; }
        public int? SupplierKey { get; set; }
        public int? Category { get; set; }
        public int? Department { get; set; }
        public string Description { get; set; }
        public bool? IsDisplay { get; set; }
        public int? CostSold { get; set; }
        public int? CostBought { get; set; }

        public int? QuantityInStorage { get; set; }

        public int? QuantityInDisplay { get; set; }
    }
}
