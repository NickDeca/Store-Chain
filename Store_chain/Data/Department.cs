using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Store_chain.Models;

namespace Store_chain.Data
{
    /// <summary>
    /// Department works with rows being what product id is connected to what department id
    /// </summary>
    public class Department
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        // Description of the Department
        public string Description { get; set; }
        public int? DepartmentKey { get; set; }

        // Number of products in display
        public int? Number { get; set; }
        public int State { get; set; }
        [DisplayName("Product Id")]
        public int Prod_Id { get; set; }
        //TODO remove
        [ForeignKey("DepartmentForeignId")]
        public List<Products> Products { get; set; }
    }
}
