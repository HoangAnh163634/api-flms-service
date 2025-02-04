﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api_flms_service.Model
{
    public class Book
    {
        [Key]
        public int BookId { get; set; } // Primary Key
        public string BookName { get; set; } = null!;
        [ForeignKey(nameof(Author))]
        public int AuthorId { get; set; } // Foreign Key
        [ForeignKey(nameof(Category))]
        public int CatId { get; set; } // Foreign Key
        public int BookNo { get; set; }
        public int BookPrice { get; set; }

        // Navigation Properties
        public Author Author { get; set; } = null!;
        public Category Category { get; set; } = null!;
    }
}
