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
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Company> CompanyList = _unitOfWork.Company.GetAll().ToList();
            
            return View(CompanyList);
        }

        //Upsert = Update + Insert
        public IActionResult Upsert(int? id)
        {
            
            if(id == null || id == 0)
            {

            }
            else
            {
                Company company = _unitOfWork.Company.Get(u => u.ID == id);
                //CompanyVM.Company = _unitOfWork.Company.Get(u => u.ID == id);
                return View(company);
            }
            
            return View(new Company());
        }

        [HttpPost]
        public IActionResult Upsert(Company obj)
        {
            if (ModelState.IsValid)
            {


                if (obj.ID == 0)
                {
                    _unitOfWork.Company.Add(obj);
                    TempData["success"] = "Company created successfully.";
                }
                else
                {
                    _unitOfWork.Company.Update(obj);
                    TempData["success"] = "Company updated successfully.";
                }
                _unitOfWork.Save();

                return RedirectToAction("Index", "Company");
            }
            else
            {
                return View(obj);
            }
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> data = _unitOfWork.Company.GetAll().ToList();
            return Json(new { data });

        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var obj = _unitOfWork.Company.Get(u=>u.ID==id);
            if (obj == null)
            {
                return Json(new { success = false, message = "Error while deleting Company." });
            }

            _unitOfWork.Company.Remove(obj);
            _unitOfWork.Save();
            return Json(new { success=true, message = "Company deleted successfully." });

        }
        #endregion
    }
}
