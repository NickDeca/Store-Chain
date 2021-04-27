using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Store_chain.Models;

namespace Store_chain.Data.Managers
{
    public abstract class BaseManager<TModel, TModelDTO> 
        where TModel : class, IBaseModel 
        where TModelDTO : class // IModelDTO
    {
        protected abstract void ChangeDTOToFull(ref TModel fullClass, TModelDTO DTO);       // na mpei se service kalutera 

    }
}
