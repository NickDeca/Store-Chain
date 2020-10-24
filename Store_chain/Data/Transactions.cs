﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store_chain.Data
{
    public class Transactions
    {
        public int Id { get; set; }
        public int RecipientKey { get; set; }
        public int ProviderKey { get; set; }
        public decimal? Capital { get; set; }
        public int? ProductKey { get; set; }
        public DateTime DateOfTransaction { get; set; }
        public int? ProductQuantity { get; set; }
        public int? State { get; set; }
        public string ErrorText { get; set; }
    }
}
