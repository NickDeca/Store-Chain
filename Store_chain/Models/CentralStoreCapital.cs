using System.ComponentModel.DataAnnotations;
using Store_chain.Models;

namespace Store_chain.Data
{
    public class CentralStoreCapital : BaseModel
    {
        public decimal Capital { get; set; }
        public int TransactionKey { get; set; }
    }
}
