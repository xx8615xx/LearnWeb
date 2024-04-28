using Learn.DataAccess.Repository.IRepository;
using Learn.Models;
using Learn.Models.ViewModels;
using Learn.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LearnWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
            var userid = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new() {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserID == userid, includeProperties: "Product"),
                OrderHeader = new()
            };

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            return View(ShoppingCartVM);
        }

        public IActionResult Summary()
        {
			ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
			var userid = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			ShoppingCartVM = new()
			{
				ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserID == userid, includeProperties: "Product"),
				OrderHeader = new()
			};

			ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userid);

			ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
			ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
			ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.Address;
			ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;

			foreach (var cart in ShoppingCartVM.ShoppingCartList)
			{
				cart.Price = GetPriceOnQuantity(cart);
				ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
			}

			return View(ShoppingCartVM);
		}

        [HttpPost]
        [ActionName("Summary")]
		public IActionResult SummaryPost()
		{
			ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
			var userid = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserID == userid, includeProperties: "Product");

			ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
			ShoppingCartVM.OrderHeader.ApplicationUserID = userid;
            ShoppingCartVM.OrderHeader.City = "notUsedNow";
            ShoppingCartVM.OrderHeader.State = "TW";

			ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userid);

			foreach (var cart in ShoppingCartVM.ShoppingCartList)
			{
				cart.Price = GetPriceOnQuantity(cart);
				ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
			}

			if (applicationUser.CompanyID.GetValueOrDefault() == 0)
			{
                //it is a regular account
				ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
				ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
			}
			else
			{
                //it is a company user
				ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
				ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
			}
			_unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
			_unitOfWork.Save();

			foreach (var cart in ShoppingCartVM.ShoppingCartList)
			{
				OrderDetail orderDetail = new()
				{
					ProductID = cart.ProductID,
					OrderHeaderID = ShoppingCartVM.OrderHeader.ID,
					Price = cart.Price,
					Count = cart.Count
				};
				_unitOfWork.OrderDetail.Add(orderDetail);
				_unitOfWork.Save();
			}

			if (applicationUser.CompanyID.GetValueOrDefault() == 0)
			{
				//it is a regular account. we need to capture payment
			}

			return RedirectToAction(nameof(OrderConfirmation), new { orderID = ShoppingCartVM.OrderHeader.ID});
		}

        public IActionResult OrderConfirmation(int orderID)
        {
            return View(orderID);
        }

		public IActionResult Plus(int cartID)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u=>u.Id == cartID);
            cartFromDb.Count++;
            _unitOfWork.ShoppingCart.Update(cartFromDb);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Minus(int cartID)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartID);
            if (cartFromDb.Count <= 1)
            {
                _unitOfWork.ShoppingCart.Remove(cartFromDb);
            }
            else
            {
                cartFromDb.Count--;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Remove(int cartID)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartID);
            _unitOfWork.ShoppingCart.Remove(cartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        private int GetPriceOnQuantity(ShoppingCart shoppingCart)
        {
            if(shoppingCart.Count < 50)
            {
                return shoppingCart.Product.Price1;
            }
            else if (shoppingCart.Count < 100)
            {
                return shoppingCart.Product.Price50;
            }
            else
            {
                return shoppingCart.Product.Price100;
            }
        }
    }
}
