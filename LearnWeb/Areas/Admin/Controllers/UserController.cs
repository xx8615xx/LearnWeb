using Learn.DataAccess.Data;
using Learn.DataAccess.Repository;
using Learn.DataAccess.Repository.IRepository;
using Learn.Models;
using Learn.Models.ViewModels;
using Learn.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LearnWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;

        public UserController(UserManager<IdentityUser> userManager, IUnitOfWork unitOfWork, RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RoleManagement(string userID)
        {
            UserManagementVM userManagementVM = new ()
            {
                ApplicationUser = _unitOfWork.ApplicationUser.Get(u=>u.Id == userID),
                RoleList = _roleManager.Roles.Select(u => u.Name).Select(x => new SelectListItem()
                {
                    Text = x,
                    Value = x
                }),

                CompanyList = _unitOfWork.Company.GetAll().Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.ID.ToString()
                })
            };
            userManagementVM.ApplicationUser.Role = _userManager.GetRolesAsync(userManagementVM.ApplicationUser).GetAwaiter().GetResult().FirstOrDefault();
            return View(userManagementVM);
        }

        [HttpPost]
        public IActionResult RoleManagement(UserManagementVM userManagementVM)
        {
            ApplicationUser applicationUser = _unitOfWork.ApplicationUser
                .Get(u => u.Id == userManagementVM.ApplicationUser.Id);
            string oldRole = _userManager.GetRolesAsync(applicationUser).GetAwaiter().GetResult().FirstOrDefault();

            if(!(oldRole== userManagementVM.ApplicationUser.Role))
            {
                // a role was updated
                if(userManagementVM.ApplicationUser.Role == SD.Role_Company)
                {
                    applicationUser.CompanyID = userManagementVM.ApplicationUser.CompanyID;
                }
                if (oldRole == SD.Role_Company)
                {
                    applicationUser.CompanyID = null;
                }
                _unitOfWork.ApplicationUser.Update(applicationUser);
                _unitOfWork.Save();

                _userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(applicationUser, userManagementVM.ApplicationUser.Role).GetAwaiter().GetResult();
            }
            else
            {
                if (oldRole == SD.Role_Company && applicationUser.CompanyID != userManagementVM.ApplicationUser.CompanyID)
                {
                    applicationUser.CompanyID = userManagementVM.ApplicationUser.CompanyID;
                    _unitOfWork.ApplicationUser.Update(applicationUser);
                    _unitOfWork.Save();
                }
            }

            return RedirectToAction(nameof(Index));
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> data = _unitOfWork.ApplicationUser.GetAll(includeProperties:"Company").ToList();
            foreach (var user in data)
            {
                user.Role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();

                if (user.Company == null)
                {
                    user.Company = new Company()
                    {
                        Name = ""
                    };
                }
            }
            return Json(new { data });

        }

        [HttpPost]
        public IActionResult LockAndUnlock([FromBody]string id)
        {
            var objFromDb = _unitOfWork.ApplicationUser.Get(u=>u.Id == id,tracked:true);
            if (objFromDb==null)
            {
                return Json(new { sucess=false,message="Error while Locking/Unlocking"});
            }

            if(objFromDb.LockoutEnd!=null && objFromDb.LockoutEnd > DateTime.Now)
            {
                // user is locked
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            _unitOfWork.Save();

            return Json(new { success=true, message = "Lock/Unlock successfully." });

        }
        #endregion
    }
}
