using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Store_chain.Models;

namespace Store_chain.Data
{
    public class Department
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int? Number { get; set; }
        public int State { get; set; }
        [ForeignKey("DepartmentForeignId")]
        public ICollection<Products> Products { get; set; }
    }
}
