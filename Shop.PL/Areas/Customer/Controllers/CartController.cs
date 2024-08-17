using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Shop.BLL.Specifications;
using Shop.BLL.UnitOfWork;
using Shop.DAL.Entities;
using Shop.PL.Helper;
using Shop.PL.ViewModels;
using Stripe.Checkout;
using System.Security.Claims;

namespace Shop.PL.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public CartController(IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            string userId = claim.Value;

            var shoppingCartSpec = new ShoppingCartSpec(userId);
            var customerCarts = await _unitOfWork.Repository<ShoppingCart, int>().GetAllWithSpecAsync(shoppingCartSpec);

            if (customerCarts is null)
                return NotFound();

            var cartVM = new ShoppingCartDetailsVM()
            {
                Carts = customerCarts,
                Total = customerCarts.Sum(c => c.Count * c.Product.Price)
            };

            if (cartVM.Total == 0)
                return RedirectToAction("Index", "Home", new { area = "Customer" });

            return View(cartVM);
        }

        public async Task<IActionResult> Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            string userId = claim.Value;

            var shoppingCartSpec = new ShoppingCartSpec(userId);
            var customerCarts = await _unitOfWork.Repository<ShoppingCart, int>().GetAllWithSpecAsync(shoppingCartSpec);
            var customer = await _userManager.FindByIdAsync(userId);

            if (customerCarts is null)
                return NotFound();

            var orderDetails = new OrderCreateVM()
            {
                Carts = customerCarts,
                CustOrderHeader = new OrderHeader()
            };
            orderDetails.CustOrderHeader.Address = customer.Address;
            orderDetails.CustOrderHeader.City = customer.City;
            orderDetails.CustOrderHeader.Name = customer.Name;
            orderDetails.CustOrderHeader.TotalPrice = orderDetails.Carts.Sum(c => c.Product.Price * c.Count);
            
            
            return View(orderDetails);
        }

        [HttpPost]
        public async Task<IActionResult> PayOrder(OrderCreateVM orderDetailsVM)
        {
            //if (!ModelState.IsValid)
            //    return BadRequest();

			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
			string userId = claim.Value;

            var shoppingCartSpec = new ShoppingCartSpec(userId);
            var customerCarts = await _unitOfWork.Repository<ShoppingCart, int>().GetAllWithSpecAsync(shoppingCartSpec);
           

            orderDetailsVM.Carts = customerCarts;
            orderDetailsVM.CustOrderHeader.OrderDate = DateTime.Now;
            orderDetailsVM.CustOrderHeader.OrderStatus = OrderStatus.Pending;
            orderDetailsVM.CustOrderHeader.PaymentStatus = PaymentStatus.Pending;
            orderDetailsVM.CustOrderHeader.TotalPrice = customerCarts.Sum(c => c.Count * c.Product.Price);
            orderDetailsVM.CustOrderHeader.ApplicationUserId = userId;
            

            await _unitOfWork.Repository<OrderHeader, int>().AddAsync(orderDetailsVM.CustOrderHeader);
            await _unitOfWork.CompleteAsync();

            foreach(var item in customerCarts)
            {
                var orderDetails = new OrderDetail()
                {
                    ProductId = item.ProductId,
                    OrderHeaderId = orderDetailsVM.CustOrderHeader.Id,
                    Count = item.Count,
                    Price = item.Product.Price
                };
                await _unitOfWork.Repository<OrderDetail, int>().AddAsync(orderDetails);
				await _unitOfWork.CompleteAsync();

			}

            var domain = "https://localhost:7105/";

			var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),

                Mode = "payment",
                SuccessUrl = domain+$"Customer/Cart/OrderConfirmation?id={orderDetailsVM.CustOrderHeader.Id}",
                CancelUrl = domain+$"Customer/Cart/Index",
            };

            foreach (var item in orderDetailsVM.Carts)
            {
                var sessionLineItemOptions = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Product.Price * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name,
                        },
                    },
                    Quantity = item.Count,
                };
                options.LineItems.Add(sessionLineItemOptions);
            }

            var service = new SessionService();
            Session session = service.Create(options);
            orderDetailsVM.CustOrderHeader.SessionId = session.Id;
            

            _unitOfWork.Repository<OrderHeader, int>().Update(orderDetailsVM.CustOrderHeader);
            await _unitOfWork.CompleteAsync();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);

        }

        public async Task<IActionResult> OrderConfirmation(int id)
        {
            var orderHeader = await _unitOfWork.Repository<OrderHeader, int>().GetByIdAsync(id);
			var service = new SessionService();
			Session session = service.Get(orderHeader.SessionId);

            if(session.PaymentStatus.ToLower() == PaymentStatus.Paid.ToLower())
            {
                orderHeader.PaymentStatus = PaymentStatus.Paid;
                orderHeader.OrderStatus = OrderStatus.Confirmed;
                orderHeader.PaymentDate = DateTime.Now;
                orderHeader.PaymentIntentId = session.PaymentIntentId;
                _unitOfWork.Repository<OrderHeader, int>().Update(orderHeader);
                await _unitOfWork.CompleteAsync();
            }

            var shoppingCartSpec = new ShoppingCartSpec(orderHeader.ApplicationUserId);
            var currentCarts = await _unitOfWork.Repository<ShoppingCart, int>().GetAllWithSpecAsync(shoppingCartSpec);

            _unitOfWork.Repository<ShoppingCart, int>().DeleteRange(currentCarts);
            await _unitOfWork.CompleteAsync();

            var cartsCount = await _unitOfWork.Repository<ShoppingCart, int>().GetAllWithSpecAsync(shoppingCartSpec);
            HttpContext.Session.SetInt32("SessionKey", cartsCount.ToList().Count());
            return View(id);
		}

        public async Task<IActionResult> Plus(int cartId)
        {
            var cart = await _unitOfWork.Repository<ShoppingCart, int>().GetByIdAsync(cartId);
            if (cart is null) return NotFound();

            cart.Count += 1;
            _unitOfWork.Repository<ShoppingCart, int>().Update(cart);
            await _unitOfWork.CompleteAsync();

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var shoppingCartSpec = new ShoppingCartSpec(claim.Value);
            var cartsCount = await _unitOfWork.Repository<ShoppingCart, int>().GetAllWithSpecAsync(shoppingCartSpec);
            HttpContext.Session.SetInt32("SessionKey", cartsCount.ToList().Count());

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Minus(int cartId)
        {
            var cart = await _unitOfWork.Repository<ShoppingCart, int>().GetByIdAsync(cartId);
            if (cart is null) return NotFound();

            if(cart.Count == 1)
            {
                _unitOfWork.Repository<ShoppingCart, int>().Delete(cart);
            }
            else
            {
                cart.Count -= 1;
                _unitOfWork.Repository<ShoppingCart, int>().Update(cart);
            }
  
            await _unitOfWork.CompleteAsync();

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var shoppingCartSpec = new ShoppingCartSpec(claim.Value);
            var cartsCount = await _unitOfWork.Repository<ShoppingCart, int>().GetAllWithSpecAsync(shoppingCartSpec);
            HttpContext.Session.SetInt32("SessionKey", cartsCount.ToList().Count());

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Remove(int cartId)
        {
            var cart = await _unitOfWork.Repository<ShoppingCart, int>().GetByIdAsync(cartId);
            if (cart is null) return NotFound();
            _unitOfWork.Repository<ShoppingCart, int>().Delete(cart);
            await _unitOfWork.CompleteAsync();

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var shoppingCartSpec = new ShoppingCartSpec(claim.Value);
            var carts = await _unitOfWork.Repository<ShoppingCart, int>().GetAllWithSpecAsync(shoppingCartSpec);
            HttpContext.Session.SetInt32("SessionKey", carts.ToList().Count());

            return RedirectToAction(nameof(Index));

        }


    }
}
