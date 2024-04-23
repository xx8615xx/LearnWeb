using LearnWebRazor.Data;
using LearnWebRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LearnWebRazor.Pages.Categories
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _db;
        [BindProperty]
        public Category category { get; set; }
        public EditModel (AppDbContext db)
        {
            _db = db;
        }
        public void OnGet(int? id)
        {
            if (id != null && id != 0)
            {
                //Category? categoryFromDB = _db.Categories.Find(id);
                //Category? categoryFromDB2 = _db.Categories.FirstOrDefault(u=>u.ID==id);
                //Category? categoryFromDB3 = _db.Categories.Where(u=>u.ID==id).FirstOrDefault();
                category = _db.Categories.Find(id);
            }
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                _db.Categories.Update(category);
                _db.SaveChanges();
                TempData["success"] = "Category edited successfully.";
                return Redirect("Index");
            }
            return Page();
        }
    }
}
