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
        public int Prod_Id { get; set; }
        //TODO make it with ef core work
        [ForeignKey("DepartmentForeignId")]
        public List<Products> Products { get; set; }
    }
}
