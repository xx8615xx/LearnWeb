using Learn.DataAccess.Repository.IRepository;
using Learn.Models;
using Learn.Models.ViewModels;
using Learn.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LearnWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> productList = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();
            
            return View(productList);
        }

        //Upsert = Update + Insert
        public IActionResult Upsert(int? id)
        {
            //ViewBag.CategoryList = categoryList;
            //ViewData["CategoryList"] = categoryList;
            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category
                    .GetAll().Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.ID.ToString()
                    }),
                Product = new Product()
            };
            if(id == null || id == 0)
            {

            }
            else
            {
                productVM.Product = _unitOfWork.Product.Get(u => u.ID == id,includeProperties: "ProductImages");
            }
            
            return View(productVM);
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM obj, List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                if (obj.Product.ID == 0)
                {
                    _unitOfWork.Product.Add(obj.Product);
                    TempData["success"] = "Product created successfully.";
                }
                else
                {
                    _unitOfWork.Product.Update(obj.Product);
                    TempData["success"] = "Product updated successfully.";
                }
                _unitOfWork.Save();

                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (files != null)
                {
                    if (obj.Product.ProductImages == null)
                        obj.Product.ProductImages = new List<ProductImage>();

                    foreach (IFormFile file in files)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string productPath = @"images\products\product_"+obj.Product.ID;
                        string finalPath = Path.Combine(wwwRootPath, productPath);

                        if (!Directory.Exists(finalPath)) Directory.CreateDirectory(finalPath);

                        using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        ProductImage productImage = new()
                        {
                            ImageUrl = @"\" + productPath + @"\" + fileName,
                            ProductID = obj.Product.ID,
                        };

                        obj.Product.ProductImages.Add(productImage);
                        //_unitOfWork.ProductImage.Add(productImage);
                    }

                    _unitOfWork.Product.Update(obj.Product);
                    _unitOfWork.Save();
                }
                return RedirectToAction("Index", "Product");
            }
            else
            {
                obj.CategoryList = _unitOfWork.Category
                    .GetAll().Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.ID.ToString()
                    });
                return View(obj);
            }
        }

        public IActionResult DeleteImage(int ImageID)
        {
            var imageToBeDelete = _unitOfWork.ProductImage.Get(u=>u.ID== ImageID);
            int productID = imageToBeDelete.ProductID;
            if (imageToBeDelete != null)
            {
                if (!string.IsNullOrEmpty(imageToBeDelete.ImageUrl))
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string oldImagePath = Path.Combine(wwwRootPath, imageToBeDelete.ImageUrl.TrimStart('\\'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                _unitOfWork.ProductImage.Remove(imageToBeDelete);
                _unitOfWork.Save();

                TempData["success"] = "Image Deleted successfully.";
            }
            return RedirectToAction(nameof(Upsert), new { id= productID });
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> data = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data });

        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var obj = _unitOfWork.Product.Get(u=>u.ID==id);
            if (obj == null)
            {
                return Json(new { success = false, message = "Error while deleting product." });
            }

            string productPath = @"images\products\product_" + id;
            string finalPath = Path.Combine(_webHostEnvironment.WebRootPath, productPath);

            if (Directory.Exists(finalPath))
            {
                string[] filePaths = Directory.GetFiles(finalPath);
                foreach (string filePath in filePaths)
                {
                    System.IO.File.Delete(filePath);
                }
                Directory.Delete(finalPath, true);
            }

            _unitOfWork.Product.Remove(obj);
            _unitOfWork.Save();
            return Json(new { success=true, message = "Product deleted successfully." });

        }
        #endregion
    }
}
