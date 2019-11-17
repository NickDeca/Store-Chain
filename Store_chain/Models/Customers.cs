using System;
using System.Collections.Generic;

namespace Store_chain.Model
{
    public partial class Customers
    {
        public int Id { get; set; }
        public decimal? Capital { get; set; }
        public string Description { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
