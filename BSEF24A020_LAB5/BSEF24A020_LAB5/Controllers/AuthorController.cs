using BSEF24A020_LAB5.Models;
using Microsoft.AspNetCore.Mvc;

namespace BSEF24A020_LAB5.Controllers
{
    public class AuthorController : Controller
    {
        private static int lastAuthorId = 2;
        public static List<Author> authors = new()
        {
            new Author { Id = 1, Name = "M.Y. Sohail", Country = "Pakistan" },
            new Author { Id = 2, Name = "A. Niaz", Country = "Bambolina" }
        };

        public static void AddAuthor(string name, string country)
        {
            //int newId = authors.Count == 0 ? 1 : authors.Last().Id + 1;
            int newId = ++lastAuthorId;

            authors.Add(new Author
            {
                Id = newId,
                Name = name,
                Country = country
            });
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
