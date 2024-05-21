using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BooksAPI.DTOs
{
    public class BookUpdateDto
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Genre { get; set; }
    }
}