using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.BLL.Specifications;
using Shop.BLL.UnitOfWork;
using Shop.DAL.Entities;
using Shop.PL.Helper;
using Shop.PL.ViewModels;
using Stripe;
using X.PagedList.Extensions;

namespace Shop.PL.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "Admin")]
	public class OrderController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public OrderController(IUnitOfWork unitOfWork,
			IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<IActionResult> Index(int? page, int? searchValue)
		{
			var pageNumber = page ?? 1;
			int pageSize = 5;

			IEnumerable<OrderHeader> orderHeaders;
			if(searchValue is not null)
			{
                var orderSpec = new OrderHeaderWithAppUserSpec(searchValue.Value);
                orderHeaders = await _unitOfWork.Repository<OrderHeader, int>().GetAllWithSpecAsync(orderSpec);
            }
			else
			{
                var orderSpec = new OrderHeaderWithAppUserSpec();
                orderHeaders = await _unitOfWork.Repository<OrderHeader, int>().GetAllWithSpecAsync(orderSpec);
            }

		
			var mappedOrders = _mapper.Map<IEnumerable<OrderHeaderVM>>(orderHeaders).ToPagedList(pageNumber, pageSize);
			return View(mappedOrders);
		}

		public async Task<IActionResult> Details(int orderId)
		{
			var orderHeaderSpec = new OrderHeaderWithAppUserSpec(orderId);
			var orderHeader = await _unitOfWork.Repository<OrderHeader, int>().GetByIdWithSpecAsync(orderHeaderSpec);
			var orderDetailsSpec = new OrderDetailsByOrderHeaderIdWithProductSpec(orderId);
			var orderDetails = await _unitOfWork.Repository<OrderDetail, int>().GetAllWithSpecAsync(orderDetailsSpec);

			var orderHeaderAndDetailsVM = new OrderHeaderAndDetailsVM()
			{
				OrderDetailVM = _mapper.Map<IEnumerable<OrderDetailVM>>(orderDetails),
				OrderHeaderVM = _mapper.Map<OrderHeaderVM>(orderHeader)
			};
			return View(orderHeaderAndDetailsVM);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> UpdateOrderDetails(OrderHeaderAndDetailsVM inputOrder)
		{
			var orderHeaderFromDb = await _unitOfWork.Repository<OrderHeader, int>().GetByIdAsync(inputOrder.OrderHeaderVM.Id);

			orderHeaderFromDb.Address = inputOrder.OrderHeaderVM.Address;
			orderHeaderFromDb.Name = inputOrder.OrderHeaderVM.Name;
			orderHeaderFromDb.PhoneNumber = inputOrder.OrderHeaderVM.PhoneNumber;
			orderHeaderFromDb.TrackingNumber = inputOrder.OrderHeaderVM.TrackingNumber;
			orderHeaderFromDb.Carrier = inputOrder.OrderHeaderVM.Carrier;
			orderHeaderFromDb.ShippingDate = inputOrder.OrderHeaderVM.ShippingDate;
			orderHeaderFromDb.City = inputOrder.OrderHeaderVM.City;

			_unitOfWork.Repository<OrderHeader, int>().Update(orderHeaderFromDb);
			await _unitOfWork.CompleteAsync();

            TempData["Updated"] = "Order Details has been updated successfully";
            return RedirectToAction("Details", new {orderId = orderHeaderFromDb.Id});
        }

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ProcessOrder(OrderHeaderAndDetailsVM inputOrder)
		{
			var orderHeaderFromDb = await _unitOfWork.Repository<OrderHeader, int>().GetByIdAsync(inputOrder.OrderHeaderVM.Id);
			orderHeaderFromDb.OrderStatus = OrderStatus.Processing;

			_unitOfWork.Repository<OrderHeader, int>().Update(orderHeaderFromDb);
			await _unitOfWork.CompleteAsync();

            TempData["Updated"] = "Order Status has been updated successfully";
            return RedirectToAction("Details", new { orderId = orderHeaderFromDb.Id });
        }

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ShipOrder(OrderHeaderAndDetailsVM inputOrder)
		{
            var orderHeaderFromDb = await _unitOfWork.Repository<OrderHeader, int>().GetByIdAsync(inputOrder.OrderHeaderVM.Id);
			orderHeaderFromDb.Carrier = inputOrder.OrderHeaderVM.Carrier;
			orderHeaderFromDb.ShippingDate = DateTime.Now;
			orderHeaderFromDb.TrackingNumber = inputOrder.OrderHeaderVM.TrackingNumber;
			orderHeaderFromDb.OrderStatus = OrderStatus.Shipped;

            _unitOfWork.Repository<OrderHeader, int>().Update(orderHeaderFromDb);
            await _unitOfWork.CompleteAsync();

            TempData["Updated"] = "Order Shipping Data has been updated successfully";
            return RedirectToAction("Details", new { orderId = orderHeaderFromDb.Id });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(OrderHeaderAndDetailsVM inputOrder)
        {
            var orderHeaderFromDb = await _unitOfWork.Repository<OrderHeader, int>().GetByIdAsync(inputOrder.OrderHeaderVM.Id);

			if(orderHeaderFromDb.PaymentStatus.ToLower() == PaymentStatus.Paid.ToLower())
			{
				var options = new RefundCreateOptions()
				{
					Reason = RefundReasons.RequestedByCustomer,
					PaymentIntent = orderHeaderFromDb.PaymentIntentId
				};

				var service = new RefundService();
				var refund = await service.CreateAsync(options);

				orderHeaderFromDb.PaymentStatus = PaymentStatus.Refunded;
			}
			else
			{
				orderHeaderFromDb.PaymentStatus = PaymentStatus.Cancelled;
			}

			orderHeaderFromDb.OrderStatus = OrderStatus.Cancelled;
            _unitOfWork.Repository<OrderHeader, int>().Update(orderHeaderFromDb);
			await _unitOfWork.CompleteAsync();

            TempData["Updated"] = "Order Status has been updated successfully";
            return RedirectToAction("Details", new { orderId = orderHeaderFromDb.Id });
        }

    }
}
