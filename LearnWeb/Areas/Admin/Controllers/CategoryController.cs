using Learn.DataAccess.Data;
using Learn.DataAccess.Repository;
using Learn.DataAccess.Repository.IRepository;
using Learn.Models;
using Microsoft.AspNetCore.Mvc;

namespace LearnWeb.Areas.Admin.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Category> categoryList = _unitOfWork.Category.GetAll().ToList();
            return View(categoryList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category obj)
        {
            //if (obj.Name == obj.DisplayOrder.ToString())
            //{
            //    ModelState.AddModelError("name","Category Name shold not be the same with Display Order");
            //}
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Category created successfully.";
                return RedirectToAction("Index", "Category");
            }
            return View();
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            //Category? categoryFromDB = _db.Categories.Find(id);
            //Category? categoryFromDB2 = _db.Categories.FirstOrDefault(u=>u.ID==id);
            //Category? categoryFromDB3 = _db.Categories.Where(u=>u.ID==id).FirstOrDefault();
            Category? categoryFromDB = _unitOfWork.Category.Get(u => u.ID == id);
            if (categoryFromDB == null)
            {
                return NotFound();
            }
            return View(categoryFromDB);
        }
        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Category edited successfully.";
                return RedirectToAction("Index", "Category");
            }
            return View();
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            //Category? categoryFromDB = _db.Categories.Find(id);
            //Category? categoryFromDB2 = _db.Categories.FirstOrDefault(u=>u.ID==id);
            //Category? categoryFromDB3 = _db.Categories.Where(u=>u.ID==id).FirstOrDefault();
            Category? categoryFromDB = _unitOfWork.Category.Get(u => u.ID == id);
            if (categoryFromDB == null)
            {
                return NotFound();
            }
            return View(categoryFromDB);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            //Category? obj = _db.Categories.Find(id);
            Category? obj = _unitOfWork.Category.Get(u => u.ID == id);
            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.Category.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Category deleted successfully.";
            return RedirectToAction("Index", "Category");
        }
    }
}
