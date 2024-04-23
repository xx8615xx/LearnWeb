using LearnWebRazor.Data;
using LearnWebRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LearnWebRazor.Categories
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _db;
        public List<Category> categoryList { get; set; }
        public IndexModel(AppDbContext db)
        {
            _db = db;
        }
       
        public void OnGet()
        {
            categoryList = _db.Categories.ToList();

        }
    }
}
