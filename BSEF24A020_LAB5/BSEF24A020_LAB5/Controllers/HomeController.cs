using BSEF24A020_LAB5.Models;
using BSEF24A020_LAB5.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BSEF24A020_LAB5.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            HomeViewModel model = new HomeViewModel();

            model.Authors = AuthorController.authors;

            foreach (Book b in BookController.books)
            {
                BookCardViewModel card = new BookCardViewModel();

                card.Id = b.Id;
                card.Title = b.Title;
                card.Price = b.Price;

                foreach (Author a in AuthorController.authors)
                {
                    if (a.Id == b.AuthorId)
                    {
                        card.AuthorName = a.Name;
                        break;
                    }
                }

                model.Books.Add(card);
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult AddAuthor(string name, string country)
        {
            AuthorController.AddAuthor(name, country);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult AddBook(string title, decimal price, int authorId)
        {
            BookController.AddBook(title, price, authorId);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult RemoveBook(int id)
        {
            Book bookToRemove = null;

            foreach (Book b in BookController.books)
            {
                if (b.Id == id)
                {
                    bookToRemove = b;
                    break;
                }
            }

            if (bookToRemove != null)
            {
                BookController.books.Remove(bookToRemove);
            }

            return RedirectToAction("Index");
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}