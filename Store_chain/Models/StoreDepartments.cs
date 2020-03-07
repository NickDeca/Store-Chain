using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Store_chain.Model
{
    public partial class StoreDepartments
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; }
        public int? MaxProducts { get; set; }

        public int ProductKey { get; set; }
    }
}
