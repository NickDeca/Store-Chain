using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Store_chain.Models
{
    public class ProductMinQuantity
    {
        [Key]
        public int id { get; }

        public int ProductKey { get; set; }
        public int MinDisplay { get; set; }
        public int MinStorage { get; set; }
    }
}
