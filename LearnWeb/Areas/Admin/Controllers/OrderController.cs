using Learn.DataAccess.Repository.IRepository;
using Learn.Models;
using Microsoft.AspNetCore.Mvc;

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
		public IActionResult GetAll()
		{
			List<OrderHeader> data = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
			return Json(new { data });

		}
		#endregion
	}
}
