using BSEF24A020_LAB5.Models;

namespace BSEF24A020_LAB5.ViewModels
{
    public class HomeViewModel
    {
        public List<BookCardViewModel> Books { get; set; } = new();
        public List<Author> Authors { get; set; } = new();
    }
}