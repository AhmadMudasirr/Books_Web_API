using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BooksAPI.DTOs
{
    public class NewBookDto
    {
        public string Title { get; set; }
        public string Genre { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int AuthorId { get; set; }
    }
}