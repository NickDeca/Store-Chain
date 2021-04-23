namespace Store_chain.Model
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Store_chain.Models;

    public abstract class BaseModel : IBaseModel
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
    }

    public abstract class BaseModelDescriptive : BaseModel
    {
        public string Description { get; set; }
    }
}
