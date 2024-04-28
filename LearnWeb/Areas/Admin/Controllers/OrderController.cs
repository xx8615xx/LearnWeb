using Learn.DataAccess.Repository.IRepository;
using Learn.Models;
using Learn.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LearnWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class OrderController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		public OrderController(IUnitOfWork unitOfWork) {
			_unitOfWork = unitOfWork;
		}
		public IActionResult Index()
		{
			return View();
		}

		#region API CALLS
		[HttpGet]
		public IActionResult GetAll(string status)
		{
			IEnumerable<OrderHeader> data = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
            switch (status)
            {
                case "pending":
					data = data.Where(u=>u.PaymentStatus==SD.PaymentStatusDelayedPayment);
                    break;
                case "inprocess":
                    data = data.Where(u => u.OrderStatus == SD.StatusInProcess);
                    break;
                case "completed":
                    data = data.Where(u => u.OrderStatus == SD.StatusShipped);
                    break;
                case "approved":
                    data = data.Where(u => u.OrderStatus == SD.StatusApproved);
                    break;
                default:
                    break;
            }

            return Json(new { data });

		}
		#endregion
	}
}
