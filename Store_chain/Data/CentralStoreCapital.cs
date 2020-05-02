using System.ComponentModel.DataAnnotations;

namespace Store_chain.Data
{
    public class CentralStoreCapital
    {
        [Key]
        public int Id { get; set; }
        public decimal Capital { get; set; }
        public int TransactionKey { get; set; }
    }
}
