using BSEF24A020_LAB5.Models;
using Microsoft.AspNetCore.Mvc;

namespace BSEF24A020_LAB5.Controllers
{
    public class BookController : Controller
    {
        private static int lastBookId = 3;
        public static List<Book> books = new()
        {
            new Book { Id = 1, Title = "Code in C#", Price = 1500, AuthorId = 1 },
            new Book { Id = 2, Title = "Code in Python", Price = 2200, AuthorId = 1 },
            new Book { Id = 3, Title = "Code in C++", Price = 1800, AuthorId = 2 }
        };

        public static void AddBook(string title, decimal price, int authorId)
        {
            if (!AuthorController.authors.Any(a => a.Id == authorId))
                return;

            //int newId = books.Count == 0 ? 1 : books.Last().Id + 1;
            int newId = ++lastBookId;

            books.Add(new Book
            {
                Id = newId,
                Title = title,
                Price = price,
                AuthorId = authorId
            });
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
