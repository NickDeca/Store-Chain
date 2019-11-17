using System;
using System.Collections.Generic;

namespace Store_chain.Model
{
    public partial class ProductCategories
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int? DepartmentKey { get; set; }
    }
}
