using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store_chain.Models
{
    public partial class Suppliers : BaseModelDescriptive
    {
        [Column("Payment_Due")]
        public decimal? PaymentDue { get; set; }
        public int? Category { get; set; }
        public string Name { get; set; }
    }
}
