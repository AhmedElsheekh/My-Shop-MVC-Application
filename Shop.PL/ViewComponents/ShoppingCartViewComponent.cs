using Microsoft.AspNetCore.Mvc;
using Shop.BLL.Specifications;
using Shop.BLL.UnitOfWork;
using Shop.DAL.Entities;
using System.Security.Claims;

namespace Shop.PL.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if(claim != null)
            {
                var cartsNum = HttpContext.Session.GetInt32("SessionKey");
                if (cartsNum != null)
                {
                    return View(cartsNum);
                }
                else
                {
                    var spec = new ShoppingCartSpec(claim.Value);
                    var carts = await _unitOfWork.Repository<ShoppingCart, int>().GetAllWithSpecAsync(spec);
                    var cartsCount = carts.ToList().Count();
                    HttpContext.Session.SetInt32("SessionKey", cartsCount);
                    return View(cartsCount);
                }
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }
        }
    }
}
