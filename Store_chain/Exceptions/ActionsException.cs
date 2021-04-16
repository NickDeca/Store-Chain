using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store_chain.Exceptions
{
    public class ActionsException :Exception
    {
        public int? CustomerId { get; }
        public int? ProductId { get; }
        public int? SupplierId { get; }

        public ActionsException() { }

        public ActionsException(string message) : base(message) { }

        public ActionsException(string message, int? customerId) : base(message)
        {
            CustomerId = customerId;
        }

        public ActionsException(string message, int? customerId, int? productId) : base(message)
        {
            CustomerId = customerId;
            ProductId = productId;
        }

        public ActionsException(string message, int? customerId, int? productId, int? supplierId) : base(message)
        {
            CustomerId = customerId;
            SupplierId = supplierId;
            ProductId = productId;
        }
    }
}
