﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BooksAPI.Models
{
    public class Book
    {

        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Genre { get; set; }

        // public DateTime PublishDate { get; set; }

        public decimal Price { get; set; }

        public string Description { get; set; }

        public int AuthorId { get; set; }

        [ForeignKey("AuthorId")]
        public Author Author { get; set; }

    }
}