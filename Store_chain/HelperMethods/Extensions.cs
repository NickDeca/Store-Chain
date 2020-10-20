using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store_chain.HelperMethods
{
    public static class Extensions
    {
        public static List<T> toListFromOne<T>(this T itemS)
        {
            return new List<T> {itemS};
        }
    }
}
