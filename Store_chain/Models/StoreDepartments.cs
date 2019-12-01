using System;
using System.Collections.Generic;

namespace Store_chain.Model
{
    public partial class StoreDepartments
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int? MaxProducts { get; set; }

        public int ProductKey { get; set; }
    }
}
