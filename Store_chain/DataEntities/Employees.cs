using System;
using System.Collections.Generic;

namespace Store_chain.Model
{
    public partial class Employees
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? DepartmentKey { get; set; }
        public bool? IsActive { get; set; }
    }
}
