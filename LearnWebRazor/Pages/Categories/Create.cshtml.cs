using LearnWebRazor.Data;
using LearnWebRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LearnWebRazor.Pages.Categories
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _db;
        [BindProperty]
        public Category category { get; set; }
        public CreateModel(AppDbContext db)
        {
            _db = db;
        }

        public void OnGet()
        {
        }
        public IActionResult OnPost() { 
            _db.Categories.Add(category);
            _db.SaveChanges();
            TempData["success"] = "Category created successfully.";
            return RedirectToPage("Index");
        }
    }
}
