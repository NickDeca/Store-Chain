using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store_chain.Model
{
    public partial class Employees
    {
        [Key]
        public int Id { get; set; }
        [Column("First_Name")]
        public string FirstName { get; set; }
        [Column("Last_Name")]
        public string LastName { get; set; }
        public int? DepartmentKey { get; set; }
        public bool? IsActive { get; set; }
    }
}
