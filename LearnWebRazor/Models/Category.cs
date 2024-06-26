﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace LearnWebRazor.Models
{
    public class Category
    {
        [Key]
        public int ID { get; set; }
        [Required]
        [MaxLength(30)]
        [DisplayName("Category Name")]
        public string Name { get; set; }
        [DisplayName("Display Order")]
        [Range(1, 100, ErrorMessage = "1~100 Please")]
        public int DisplayOrder { get; set; }

    }
}
