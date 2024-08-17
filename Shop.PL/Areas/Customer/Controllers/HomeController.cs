using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.BLL.Specifications;
using Shop.BLL.UnitOfWork;
using Shop.DAL.Entities;
using Shop.PL.ViewModels;
using Stripe;
using System.Security.Claims;
using X.PagedList.Extensions;

namespace Shop.PL.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public HomeController(IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(int? page, string? searchValue)
        {
            var pageNum = page ?? 1;
            int pageSize = 4;

            IEnumerable<Shop.DAL.Entities.Product> products;
            if(!string.IsNullOrEmpty(searchValue))
            {
                var productSpec = new ProductWithCategorySpec(searchValue);
                products = await _unitOfWork.Repository<Shop.DAL.Entities.Product, int>().GetAllWithSpecAsync(productSpec);
            }
            else
            {
                products = await _unitOfWork.Repository<Shop.DAL.Entities.Product, int>().GetAllAsync();
            }

            
            var mappedProducts = _mapper.Map<IEnumerable<ProductDetailsVM>>(products).ToPagedList(pageNum, pageSize);
            return View(mappedProducts);
        }

        public async Task<IActionResult> Details(int id)
        {

            var spec = new ProductWithCategorySpec(id);
            var product = await _unitOfWork.Repository<Shop.DAL.Entities.Product, int>().GetByIdWithSpecAsync(spec);
            var basketItem = _mapper.Map<BasketItemVM>(product);

            var productSpec = new ProductByCategoryNameSpec(product.Category.Name);
            var relatedItems = await _unitOfWork.Repository<Shop.DAL.Entities.Product, int>().GetAllWithSpecAsync(productSpec);
            

            basketItem.RelatedItems = _mapper.Map<IEnumerable<ProductDetailsVM>>(relatedItems.Count() >= 4 ? relatedItems.Take(4) : relatedItems);
            return View(basketItem);
        }

        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AddToCart(BasketItemVM input)
        {
            if (input.Count <= 0)
            {
                ModelState.AddModelError("", "Quantity Should Be Between 1 and 100");
                return BadRequest();
            }

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            string userId = claim.Value;

            var spec = new ShoppingCartSpec(userId, input.Id);
            var oldCart = await _unitOfWork.Repository<ShoppingCart, int>().GetByIdWithSpecAsync(spec);

            if(oldCart is not null)
            {
                oldCart.Count += input.Count;
                _unitOfWork.Repository<ShoppingCart, int>().Update(oldCart);
            }
            else
            {
                var shoppingCart = new ShoppingCart()
                {
                    ApplicationUserId = userId,
                    Count = input.Count,
                    ProductId = input.Id
                };
                await _unitOfWork.Repository<ShoppingCart, int>().AddAsync(shoppingCart);
            }
            
            await _unitOfWork.CompleteAsync();

            var shoppingCartSpec = new ShoppingCartSpec(claim.Value);
            var cartsCount = await _unitOfWork.Repository<ShoppingCart, int>().GetAllWithSpecAsync(shoppingCartSpec);
            HttpContext.Session.SetInt32("SessionKey", cartsCount.ToList().Count());

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> MyOrders(int? page)
        {
            var pageNumber = page ?? 1;
            int pageSize = 5;

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var orderHeaderSpec = new OrderHeaderWithAppUserSpec(claim.Value);
            var currentCustomerOrders = await _unitOfWork.Repository<OrderHeader, int>().GetAllWithSpecAsync(orderHeaderSpec);
            var mappedOrders = _mapper.Map<IEnumerable<OrderHeaderVM>>(currentCustomerOrders).ToPagedList(pageNumber, pageSize);

            return View(mappedOrders);
        }

    }
}
