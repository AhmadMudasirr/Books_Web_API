using BooksAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BooksAPI.DTOs
{
    public class BookDetailDto
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public string Genre { get; set; }

        //   public DateTime PublishDate { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public int AuthorId { get; set; }

        public string Author { get; set; }

    }
}