namespace Store_chain.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Store_chain.Models;

    internal interface IBaseController<TEntity, TModelDTO> where TEntity : class, IBaseModel
    {
        Task<IActionResult> Details(int? id);
    }
}
