using System;
using System.Collections.Generic;

namespace Store_chain.Model
{
    public partial class Products
    {
        public int Id { get; set; }
        public int? SupplierKey { get; set; }
        public int? Category { get; set; }
        public int? Department { get; set; }
        public string Description { get; set; }
        public bool? IsDisplay { get; set; }
    }
}
