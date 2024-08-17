using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shop.BLL.Specifications;
using Shop.BLL.UnitOfWork;
using Shop.DAL.Entities;
using Stripe.Climate;

namespace Shop.PL.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var allOrders = await _unitOfWork.Repository<OrderHeader, int>().GetAllAsync();
            ViewBag.AllOrders = allOrders.ToList().Count();

            var orderSpec = new OrderHeaderWithPendingStatusSpec();
            var pendingOrders = await _unitOfWork.Repository<OrderHeader, int>().GetAllWithSpecAsync(orderSpec);
            ViewBag.PendingOrders = pendingOrders.ToList().Count();

            ViewBag.Users = _userManager.Users.Count();

            var products = await _unitOfWork.Repository<Shop.DAL.Entities.Product, int>().GetAllAsync();
            ViewBag.Products = products.Count();
            return View();
        }
    }
}
