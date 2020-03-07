using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Store_chain.Models
{
    public class Store
    {
        [Key]
        public int Id { get; set; }
        public decimal Capital { get; set; }
        public DateTime TimeOfTransaction { get; set; }
        public int TransactionKey { get; set; }
    }
}
