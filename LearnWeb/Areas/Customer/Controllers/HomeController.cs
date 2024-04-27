using Learn.DataAccess.Repository;
using Learn.DataAccess.Repository.IRepository;
using Learn.Models;
using Microsoft.AspNetCore.Authorization;
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
            IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category");
            return View(productList);
        }

        public IActionResult Details(int productID)
        {
            ShoppingCart shoppingCart = new()
            {
                Product = _unitOfWork.Product.Get(u => u.ID == productID, includeProperties: "Category"),
                Count = 1,
                ProductID = productID
            };
            return View(shoppingCart);
        }
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
            var userid = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.ApplicationUserID = userid;

            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(u=>u.ProductID==shoppingCart.ProductID && u.ApplicationUserID==shoppingCart.ApplicationUserID);

            if(cartFromDb != null)
            {
                //update
                cartFromDb.Count += shoppingCart.Count;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
            }
            else
            {
                //add
                _unitOfWork.ShoppingCart.Add(shoppingCart);
            }
            TempData["success"] = "Cart updated succesfully.";
            _unitOfWork.Save();


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
