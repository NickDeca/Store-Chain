using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Store_chain.Models;

namespace Store_chain.Data.Managers
{
    public abstract class BaseManager<TModel> where TModel : class, IBaseModel
    {
        //private protected void ChangeDTOToFull(ref TModel fullClass, dynamic DTO)
        //{
        //}

    }
}
