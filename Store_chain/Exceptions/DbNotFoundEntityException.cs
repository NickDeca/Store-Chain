using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store_chain.Exceptions
{
    public class DbNotFoundEntityException : Exception
    {
        public DbNotFoundEntityException() { }
        public DbNotFoundEntityException(string message) : base(message) { }
    }
}
