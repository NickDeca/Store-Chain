using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store_chain.Model
{
    public partial class Customers
    {
        [Key]
        public int Id { get; set; }
        public decimal? Capital { get; set; }
        public string Description { get; set; }
        [Column("First_Name")]
        public string FirstName { get; set; }
        [Column("Last_Name")]
        public string LastName { get; set; }
    }
}
