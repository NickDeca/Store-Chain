using System;
using System.Collections.Generic;

namespace Store_chain.Model
{
    public partial class Suppliers
    {
        public int Id { get; set; }
        public decimal? PaymentDue { get; set; }
        public int? Category { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
    }
}
