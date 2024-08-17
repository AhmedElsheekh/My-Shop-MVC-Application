using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.BLL.UnitOfWork;
using Shop.DAL.Entities;
using Shop.PL.ViewModels;
using System.Security.Claims;
using X.PagedList.Extensions;

namespace Shop.PL.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UsersController(UserManager<ApplicationUser> userManager,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(int? page, string? searchValue)
        {
            var pageNumber = page ?? 1;
            int pageSize = 5;

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            string userId = claim.Value;

            IEnumerable<ApplicationUser> usersList;
            if(!string.IsNullOrEmpty(searchValue))
            {
                usersList = await _userManager.Users.Where(u => u.Id != userId && u.Name.ToLower().Contains(searchValue.ToLower())).ToListAsync();
            }
            else
            {
                usersList = await _userManager.Users.Where(x => x.Id != userId).ToListAsync();
            }

            var mappedUsers = _mapper.Map<IEnumerable<UserDetailsVM>>(usersList).ToPagedList(pageNumber, pageSize);
            return View(mappedUsers);
        }

        public async Task<IActionResult> LockUnlock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
                return NotFound();

            if (user.LockoutEnd == null || user.LockoutEnd < DateTime.Now)
                user.LockoutEnd = DateTime.Now.AddYears(1);
            else
                user.LockoutEnd = DateTime.Now;

            await _unitOfWork.CompleteAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
