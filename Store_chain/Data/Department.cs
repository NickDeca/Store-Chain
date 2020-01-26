using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store_chain.Data
{
    public class Department
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int? DepartmentKey { get; set; }
        public int? Number { get; set; }
        public int State { get; set; }
    }
}
