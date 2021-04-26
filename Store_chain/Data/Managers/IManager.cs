using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Store_chain.Models;

namespace Store_chain.Data.Managers
{
    public interface IManager<TModel, TModelDTO> where TModel : class, IBaseModel
    {
        Task<List<TModel>> BringAll();
        Task<TModel> BringOne(int id);
        Task<TModel> TryBringOne(int id);
        Task<TModel> FindOne(int id);
        bool Any(int id);
        Task Create(TModel model);
        Task Edit(int id, dynamic DTO);
        Task Delete(int id);
        Task<TModelDTO> FindOneDTO(int value);
        //void ChangeDTOToFull(ref TModel fullClass, dynamic DTO);
    }

}
