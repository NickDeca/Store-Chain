using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store_chain.Exceptions
{
    public class ConnectionExceptions : Exception
    {
        public int ProductId { get; }

        public ConnectionExceptions(string message) : base(message)
        {

        }

        public ConnectionExceptions(string message, int productId) : this(message)
        {
            this.ProductId = productId;
        }
    }
}
