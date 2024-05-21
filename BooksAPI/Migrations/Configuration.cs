/*namespace BooksAPI.Migrations
{
    using BooksAPI.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BooksAPI.Data.BooksAPIContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(BooksAPI.Data.BooksAPIContext context)
        {

            context.Authors.AddOrUpdate(new Author[] {

                new Author(){ AuthorId = 1 , Name = "Mudasir Ahamd"},
                new Author(){ AuthorId = 2 , Name = " Sharik"},
                new Author(){ AuthorId = 3 , Name = "Prop Inayat"}
            });

            context.Books.AddOrUpdate(new Book[] {

            new Book(){ Id = 101, Title= "Midnight Rain", Genre = "Fantasy",
            AuthorId = 1, Description =
              "A former architect battles an evil sorceress.", Price = 14.95M },

            new Book() {Id =102 ,  Title= "Sunny Day", Genre = "Hotness",
             AuthorId = 1, Description =
            "A former architect battles an evil sorceress.", Price = 10.95M},

             new Book() { Id = 5, Title = "Splish Splash", Genre = "Romance",
            AuthorId = 3, Description =
            "A deep sea diver finds true love 20,000 leagues beneath the sea.", Price = 6.99M}

            });
        }
    }
}
*/