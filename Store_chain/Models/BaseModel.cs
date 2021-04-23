namespace Store_chain.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public abstract class BaseModel
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
