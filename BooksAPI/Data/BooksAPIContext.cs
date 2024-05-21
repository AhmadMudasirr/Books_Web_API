using BooksAPI.DTOs;
using BooksAPI.Encryption;
using BooksAPI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BooksAPI.Data
{

    public class BooksAPIContext
    {
        readonly SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString);


        public async Task<List<BookDto>> Get()
        {
            string spGetBook = "[BooksAPIContext-20240420220841].dbo.spGetBooks";
            List<BookDto> list = new List<BookDto>();

            try
            {
                SqlCommand cmd = new SqlCommand(spGetBook, con);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                await Task.Run(() => adp.Fill(dt));

                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new BookDto
                    {
                        Title = Convert.ToString(dr[1]),
                        Genre = Convert.ToString(dr[2]),
                        AuthorId = Convert.ToInt32(dr[5])
                    });
                }
            }
            catch (Exception e)
            {
                throw new ApplicationException("An error occurred while fetching books", e);
            }
            return list;

        }


        public async Task<Book> GetById(int id)
        {
            string spGetBookById = "[BooksAPIContext-20240420220841].dbo.spGetBookById";
            var book = new Book();
            var auth = new Author();

            SqlCommand cmd = new SqlCommand(spGetBookById, con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", id);

            SqlDataAdapter adp = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();
            await Task.Run(() => adp.Fill(dt));

            foreach (DataRow dr in dt.Rows)
            {
                book.Id = Convert.ToInt32(dr[0]);
                book.Title = Convert.ToString(dr[1]);
                book.Genre = Convert.ToString(dr[2]);
                book.Price = Convert.ToInt32(dr[3]);
                book.Description = Convert.ToString(dr[4]);
                book.AuthorId = Convert.ToInt32(dr[5]);
            }
            return book;
        }


        public async Task<int> Create(string title, string genre, decimal price, string description, int authorId)
        {

            if (con.State != ConnectionState.Open)
            {

                con.Open();
            }

            string spAddBook = "[BooksAPIContext-20240420220841].dbo.spAddBook";

            // title = DataEncryption.Decrypt(Convert.FromBase64String(title)); ///// Decyption Testing while getting data from controller.

            SqlCommand cmd = new SqlCommand(spAddBook, con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Title", title);
            cmd.Parameters.AddWithValue("@Genre", genre);
            cmd.Parameters.AddWithValue("@Price", price);
            cmd.Parameters.AddWithValue("@Description", description);
            cmd.Parameters.AddWithValue("@AuthorId", authorId);

            int rows = await Task.Run(() => cmd.ExecuteNonQuery());
            con.Close();

            return rows;
        }



        public async Task<int> UpdateBook(int id, string title, string genre, decimal price, string description)
        {

            if (con.State != ConnectionState.Open)
            {
                con.Open();
            }

            string spUpdateBook = "[BooksAPIContext-20240420220841].dbo.spUpdateBook";

            SqlCommand cmd = new SqlCommand(spUpdateBook, con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", id); // Mandatory Parameter
            cmd.Parameters.AddWithValue("@Title", title); // Mandatory Parameter
            cmd.Parameters.AddWithValue("@Genre", genre); //Optional Parameter
            cmd.Parameters.AddWithValue("@Price", price); //Optional Parameter
            cmd.Parameters.AddWithValue("@Description", description); // Optional Parameter

            int affectedRows = await Task.Run(() => cmd.ExecuteNonQuery());
            con.Close();

            return affectedRows;
        }



        public async Task<int> DeleteBook(int id)
        {

            if (con.State != ConnectionState.Open)
            {

                con.Open();
            }

            string spDeleteBook = "[BooksAPIContext-20240420220841].dbo.spDeleteBook";

            SqlCommand cmd = new SqlCommand(spDeleteBook, con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", id);

            int affectedRows = await Task.Run(() => cmd.ExecuteNonQuery());
            con.Close();

            return affectedRows;
        }

    }
}


///////////////////////////////////////////////////////// CODE WITH ENTITY FRAMEWORK ////////////////////////////////////////////////////////////


/*  public class BooksAPIContext : DbContext
  {
      // You can add custom code to this file. Changes will not be overwritten.
      // 
      // If you want Entity Framework to drop and regenerate your database
      // automatically whenever you change your model schema, please use data migrations.
      // For more information refer to the documentation:
      // http://msdn.microsoft.com/en-us/data/jj591621.aspx

      public BooksAPIContext() : base("name=BooksAPIContext")
      {
      }

      public System.Data.Entity.DbSet<BooksAPI.Models.Book> Books { get; set; }

      public System.Data.Entity.DbSet<BooksAPI.Models.Author> Authors { get; set; }
  }*/