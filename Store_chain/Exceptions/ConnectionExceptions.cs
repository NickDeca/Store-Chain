using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store_chain.Exceptions
{
    public class ConnectionExceptions : Exception
    {
        public ConnectionExceptions(string message) : base(message)
        {

        }

    }
}
