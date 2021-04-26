using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store_chain.Data.DTO
{
    public class CustomerEditViewDTO
    {
        public string Description { get; set; }
        public decimal? Capital { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
