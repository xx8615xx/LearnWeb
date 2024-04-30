using Learn.DataAccess.Repository;
using Learn.DataAccess.Repository.IRepository;
using Learn.Models;
using Learn.Models.ViewModels;
using Learn.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace LearnWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category,ProductImages");
            return View(productList);
        }

        public IActionResult Details(int productID)
        {
            ShoppingCart shoppingCart = new()
            {
                Product = _unitOfWork.Product.Get(u => u.ID == productID, includeProperties: "Category,ProductImages"),
                Count = 1,
                ProductID = productID
            };
            return View(shoppingCart);
        }
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Count <= 0)
            {
                shoppingCart.Product = _unitOfWork.Product.Get(u => u.ID == shoppingCart.ProductID, includeProperties: "Category,ProductImages");
                shoppingCart.Count = 1;
                TempData["error"] = "Update Shopping Cart Failed.";
                return View(shoppingCart);
            }

            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
            var userid = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.ApplicationUserID = userid;

            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(u=>u.ProductID==shoppingCart.ProductID && u.ApplicationUserID==shoppingCart.ApplicationUserID);

            if(cartFromDb != null)
            {
                //update
                cartFromDb.Count += shoppingCart.Count;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
                _unitOfWork.Save();
            }
            else
            {
                //add
                _unitOfWork.ShoppingCart.Add(shoppingCart);
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart
                    .GetAll(u => u.ApplicationUserID == shoppingCart.ApplicationUserID).Count());
            }
            TempData["success"] = "Cart updated succesfully.";
            return RedirectToAction(nameof(Index));
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
