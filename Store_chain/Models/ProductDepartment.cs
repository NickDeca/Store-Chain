﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Store_chain.Models;

namespace Store_chain.Model
{
    public partial class ProductDepartment
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; }
        public int? DepartmentKey { get; set; }
        public int? Number { get; set; }
        public int State { get; set; }
        public List<Products> Products { get; set; }
    }
}
