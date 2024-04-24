using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Learn.Models
{
    public class Product
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(2000)]
        public string Description { get; set; }
        
        [Required]
        [MaxLength(40)]
        public string ISBN { get; set; }

        [Required]
        [MaxLength(100)]
        public string Author {  get; set; }

        [Required]
        [DisplayName("List Price")]
        [Range(0, 10000, ErrorMessage = "Price should be between 0 and 10000.")]
        public int ListPrice { get; set; }

        [Required]
        [DisplayName("Price for 1-49")]
        [Range(0, 10000, ErrorMessage = "Price should be between 0 and 10000.")]
        public int Price1 { get; set; }

        [Required]
        [DisplayName("Price for 50-99")]
        [Range(0, 10000, ErrorMessage = "Price should be between 0 and 10000.")]
        public int Price50 { get; set; }

        [Required]
        [DisplayName("Price for 100+")]
        [Range(0, 10000, ErrorMessage = "Price should be between 0 and 10000.")]
        public int Price100 { get; set; }
        public string ImageUrl { get; set; }
        public int CategoryID {  get; set; }
        [ValidateNever]
        [ForeignKey("CategoryID")]
        public Category Category { get; set; }
    }
}
