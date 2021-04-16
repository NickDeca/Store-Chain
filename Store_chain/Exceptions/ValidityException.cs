using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store_chain.Exceptions
{
    public class ValidityException : Exception
    {
        public ValidityException(string message) : base (message)
        {

        }
    }
}
