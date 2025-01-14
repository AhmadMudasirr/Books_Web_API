﻿using BooksAPI.Data;
using BooksAPI.DTOs;
using BooksAPI.Encryption;
using BooksAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace BooksAPI.Controllers
{

    [RoutePrefix("api/books")]
    public class BooksController : ApiController
    {
        private BooksAPIContext db = new BooksAPIContext();

        //https://localhost:44381/api/books/
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetBook()
        {
            List<BookDto> bookdata = await db.Get();

            string bookDataSerialized = JsonConvert.SerializeObject(bookdata);
            string encryptedBookData = DataEncryption.Encrypt(bookDataSerialized);

            return Content(System.Net.HttpStatusCode.OK, encryptedBookData, new JsonMediaTypeFormatter());
        }


        //https://localhost:44381/api/books/ID
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> GetBook(int id)
        {
            Book book = await db.GetById(id);

            string bookDataSerialized = JsonConvert.SerializeObject(book);
            string encryptedBookData = DataEncryption.Encrypt(bookDataSerialized);

            if (string.IsNullOrEmpty(encryptedBookData))
            {

                return BadRequest("Unable to Fetch Data!.");

            }
            else
            {
                return Content(System.Net.HttpStatusCode.OK, encryptedBookData, new JsonMediaTypeFormatter());
            }
        }



        //https://localhost:44381/api/books/createBook
        [HttpPost]
        [Route("createBook")]
        public async Task<IHttpActionResult> CreateBook()
        {
            string encryptedJsonData;
            using (var reader = new StreamReader(await Request.Content.ReadAsStreamAsync(), Encoding.UTF8))
            {
                encryptedJsonData = await reader.ReadToEndAsync();
            }

            if (string.IsNullOrEmpty(encryptedJsonData))
            {
                return BadRequest("Invalid Data. Null Passed");
            }

            string decryptedJsonData = DataEncryption.Decrypt(Convert.FromBase64String(encryptedJsonData));

            NewBookDto jsonNewBookDeserialized = JsonConvert.DeserializeObject<NewBookDto>(decryptedJsonData);

            int data = await db.Create(
                jsonNewBookDeserialized.Title,
                jsonNewBookDeserialized.Genre,
                jsonNewBookDeserialized.Price,
                jsonNewBookDeserialized.Description,
                jsonNewBookDeserialized.AuthorId);

            if (data == 0)
            {
                return Content(System.Net.HttpStatusCode.BadRequest, "Unable to Add New Book. Please check request body!", new JsonMediaTypeFormatter());
            }
            else
            {
                string encryptedResponse = DataEncryption.Encrypt(jsonNewBookDeserialized.Title);
                return Content(System.Net.HttpStatusCode.OK, "New Book Added with Title : " + encryptedResponse + ". ", new JsonMediaTypeFormatter());
            }
        }



        //https://localhost:44381/api/books/ID
        [HttpPut]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> UpdateBook([FromUri] int id)
        {
            string encryptedJsonText;
            using (var reader = new StreamReader(await Request.Content.ReadAsStreamAsync(), Encoding.UTF8))
            {
                encryptedJsonText = await reader.ReadToEndAsync();
            }

            if (string.IsNullOrEmpty(encryptedJsonText))
            {
                return BadRequest("Invalid Data. Null Passed");
            }

            string decryptedJsonData = DataEncryption.Decrypt(Convert.FromBase64String(encryptedJsonText));

            NewBookUpdateDto jsonNewBookUpdateDeserialized = JsonConvert.DeserializeObject<NewBookUpdateDto>(decryptedJsonData);

            int data = await db.UpdateBook(
                id,
                jsonNewBookUpdateDeserialized.Title,
                jsonNewBookUpdateDeserialized.Genre,
                jsonNewBookUpdateDeserialized.Price,
                jsonNewBookUpdateDeserialized.Description);

            if (data == 0)
            {
                return Content(System.Net.HttpStatusCode.NotFound, "Book having ID : " + id + " Not Found!");
            }
            else
            {
                return Content(System.Net.HttpStatusCode.OK, "Book having ID :" + id + " Updated Successfully ! ", new JsonMediaTypeFormatter());
            }
        }



        //https://localhost:44381/api/books/ID
        [HttpDelete]
        [Route("{id:int}")]

        public async Task<IHttpActionResult> DeleteBook(int id)
        {
            int data = await db.DeleteBook(id);

            if (data == 0)
            {
                return Content(System.Net.HttpStatusCode.NotFound, "Book having ID : " + id + " Not Found!");
            }
            else
            {
                return Content(System.Net.HttpStatusCode.OK, "Book with ID : " + id + ", Delete Successfully.", new JsonMediaTypeFormatter());
            }
        }
    }
}


/////////////////////////////////////////////////////CODE WITH ENTITY FRAMEWORK////////////////////////////////////////////////////////


// Typed lambda expression for Select() method. 
/* private static readonly Expression<Func<Book, BookDto>> AsBookDto =
     x => new BookDto
     {
         Title = x.Title,
         Author = x.Author.Name,
         Genre = x.Genre
     };*/

// GET api/Books
// [Route("")]
//This will Return smiple XML
/*  public IQueryable<BookDto> GetBooks()
  {
      return db.Books.Include(b => b.Author).Select(AsBookDto);
  }

  // GET api/Books/5
  [Route("{id:int}")]
  [ResponseType(typeof(BookDto))]
  //This Method will return Data is Json format with the help of JsonMediaTypeFormatter Method
  public async Task<IHttpActionResult> GetBook(int id)
  {

      BookDto book = await db.Books.Include(b => b.Author)
     .Where(b => b.Id == id)
     .Select(AsBookDto)
     .FirstOrDefaultAsync();
      if (book == null)
      {
          return NotFound();
      }

      return Content(System.Net.HttpStatusCode.OK, book, new JsonMediaTypeFormatter());

  }

  [Route("{id:int}/details")]
  [ResponseType(typeof(BookDetailDto))]
  //This Method will return Data is Json format with the help of JsonMediaTypeFormatter Method
  public async Task<IHttpActionResult> GetBookDetails(int id) // IHttpActionResult returning HTTP responses from Web API controller actions
  {

      var book = await (from b in db.Books.Include(b => b.Author)
                        where b.Id == id
                        select new BookDetailDto
                        {
                            Id = b.Id,
                            Title = b.Title,
                            Genre = b.Genre,
                            Price = b.Price,
                            Description = b.Description,
                            Author = b.Author.Name,
                            AuthorId = b.AuthorId

                        }).FirstOrDefaultAsync();

      if (book == null)
      {
          return NotFound();
      }

      return Content(System.Net.HttpStatusCode.OK, book, new JsonMediaTypeFormatter());
  }

  [HttpPost]
  [Route("create")]
  public async Task<IHttpActionResult> CreateBook([FromBody] BookCreateDto bookDto)
  {

      if (!ModelState.IsValid)
      {

          return BadRequest(ModelState);

      }

      var author = await db.Authors.FindAsync(bookDto.AuthorId); // Assuming AuthorId is provided in the bookDto
      if (author == null)
      {
          return BadRequest("Invalid author ID"); // Handle case where author ID is not valid
      }


      var book = new Book
      {
          // Id = bookDto.Id,
          Title = bookDto.Title,
          Genre = bookDto.Genre,
          AuthorId = bookDto.AuthorId
      };

      db.Books.Add(book);

      await db.SaveChangesAsync();

      var createdBookDto = new BookDetailDto
      {
          Title = book.Title,
          Genre = book.Genre,
          // Map other properties as needed
      };

      //  return CreatedAtRoute("DefaultApi", new { id = book.Id }, book);
      return CreatedAtRoute("DefaultApi", new { controller = "Books", id = book.Id }, createdBookDto);
  }

  [HttpPut]
  [Route("{id:int}")]
  public async Task<IHttpActionResult> UpdateBook(int id, [FromBody] BookUpdateDto bookDto)
  {
      if (!ModelState.IsValid)
      {

          return BadRequest();
      }

      var existingBook = await db.Books.FindAsync(id);
      if (existingBook == null)
      {
          return NotFound();
      }

      existingBook.Title = bookDto.Title;
      existingBook.Genre = bookDto.Genre;

      db.Entry(existingBook).State = EntityState.Modified;
      await db.SaveChangesAsync();

      return Content(System.Net.HttpStatusCode.OK, existingBook, new JsonMediaTypeFormatter());

  }

  /* ////////////////////---Development in Process for Patch Method--//////////////////////  
   *  public async Task<IHttpActionResult> UpdateBookPart(int id, [FromBody])
     {
         return Ok();
     }
  */


/*
[HttpDelete]
[Route("{id:int}")]
public async Task<IHttpActionResult> DeleteBook(int id)
{
    var deleteBook = await db.Books.FindAsync(id);
    if (deleteBook == null)
    {

        return NotFound();
    }

    db.Books.Remove(deleteBook);
    await db.SaveChangesAsync();

    return Content(System.Net.HttpStatusCode.OK, deleteBook, new JsonMediaTypeFormatter());

}

protected override void Dispose(bool disposing)
{
    db.Dispose();
    base.Dispose(disposing);
}*/
