using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store_chain.Data.DTO
{
    public class SupplierViewDTO
    {
        public decimal? PaymentDue { get; set; }
        public string Description { get; set; }
        public int? Category { get; set; }
        public string Name { get; set; }
    }
}
