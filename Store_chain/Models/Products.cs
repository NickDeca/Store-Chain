using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;
using Store_chain.Model;

namespace Store_chain.Models
{
    public partial class Products : BaseModelDescriptive
    {
        [Column("Supplier_Key")]
        [DisplayName("Supplier")]
        public int SupplierKey { get; set; }
        public int Category { get; set; }
        public int Department { get; set; }
        //[DisplayName("In Display")]
        //public bool IsDisplay { get; set; }

        [Column("CostSold")]
        [DisplayName("Cost Sold")]
        public decimal SoldToCustomersCost { get; set; }

        private string _soldToCustomersCost { get; set; }

        [BindProperty(Name = "SoldToCustomersCost", SupportsGet = false)]
        [NotMapped]
        public string SoldToCustomersCostAsString
        {
            get => _soldToCustomersCost;
            set
            {
                decimal.TryParse(value, out decimal trueValue);
                SoldToCustomersCost = trueValue;
            }
        }

        [Column("CostBought")]
        [DisplayName("Cost Bought")]
        public decimal BoughtFromSuppliersCost { get; set; }

        private string _BoughtFromSuppliersCost { get; set; }

        [BindProperty(Name = "BoughtFromSuppliersCost", SupportsGet = false)]
        [NotMapped]
        public string BoughtFromSuppliersCostAsString
        {
            get => _BoughtFromSuppliersCost;
            set
            {
                decimal.TryParse(value, out decimal trueValue);
                BoughtFromSuppliersCost = trueValue;
            }
        }


        [DisplayName("Quantity")]
        public int TransactionQuantity { get; set; }
        [DisplayName("Storage Quantity")]
        public int QuantityInStorage { get; set; }
        [DisplayName("Display Quantity")]
        public int QuantityInDisplay { get; set; }

        [DisplayName("Maximum Display Quantity")]
        public int MaxDisplay { get; set; }
        [DisplayName("Minimum Storage Quantity")]
        public int MinStorage { get; set; }

        public Department department { get; set; }

        public int DepartmentForeignId { get; set; }

    }
}
