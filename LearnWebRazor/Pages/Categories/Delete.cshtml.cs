using LearnWebRazor.Data;
using LearnWebRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LearnWebRazor.Pages.Categories
{
    public class DeleteModel : PageModel
    {
        private readonly AppDbContext _db;
        [BindProperty]
        public Category category { get; set; }
        public DeleteModel(AppDbContext db) {  
            _db = db; 
        }

        public IActionResult OnGet(int? id)
        {
            if(id == null || id == 0)
            {
                return NotFound();
            }
            category = _db.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            return Page();
        }

        public IActionResult OnPost()
        {
            Category? obj = _db.Categories.Find(category.ID);
            if (obj == null)
            {
                return NotFound();
            }
            _db.Categories.Remove(obj);
            _db.SaveChanges();
            TempData["success"] = "Category deleted successfully.";
            return RedirectToPage("/Categories/Index");
        }
    }
}
