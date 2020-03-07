using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store_chain.Model
{
    public partial class Suppliers
    {
        [Key]
        public int Id { get; set; }
        [Column("Payment_Due")]
        public decimal? PaymentDue { get; set; }
        public int? Category { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
    }
}
