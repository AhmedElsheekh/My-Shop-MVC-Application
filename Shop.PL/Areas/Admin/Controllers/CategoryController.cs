using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Shop.BLL.Specifications;
using Shop.BLL.UnitOfWork;
using Shop.DAL.Entities;
using Shop.PL.ViewModels;
using X.PagedList.Extensions;

namespace Shop.PL.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryController(IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(int? page, string? searchValue)
        {
            var pageNumber = page ?? 1;
            int pageSize = 5;

            IEnumerable<Category> categories;
            if (!string.IsNullOrEmpty(searchValue))
            {
                var categorySpec = new CategoryByNameSpec(searchValue);
                categories = await _unitOfWork.Repository<Category, int>().GetAllWithSpecAsync(categorySpec);
            }
            else
            {
                categories = await _unitOfWork.Repository<Category, int>().GetAllAsync();
            }

            var categoriesVM = _mapper.Map<IEnumerable<CategoryViewModel>>(categories).ToPagedList(pageNumber, pageSize);
            return View(categoriesVM);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryViewModel categoryVM)
        {
            if (ModelState.IsValid)
            {
                Category category = _mapper.Map<Category>(categoryVM);

                try
                {
                    await _unitOfWork.Repository<Category, int>().AddAsync(category);
                    await _unitOfWork.CompleteAsync();
                    TempData["Created"] = "Category has been created successfully";
                    return RedirectToAction(nameof(Index));

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(categoryVM);
        }

        public async Task<IActionResult> Details(int id, string viewName = "Details")
        {
            var category = await _unitOfWork.Repository<Category, int>().GetByIdAsync(id);
            if (category is null)
                return NotFound($"Category is not found with id = {id}");

            var categoryVM = _mapper.Map<CategoryViewModel>(category);
            return View(viewName, categoryVM);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            return await Details(id, "Update");
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromRoute] int id, CategoryViewModel categoryVM)
        {
            if (id != categoryVM.Id)
                return BadRequest();

            if (ModelState.IsValid)
            {
                var category = _mapper.Map<Category>(categoryVM);

                try
                {
                    _unitOfWork.Repository<Category, int>().Update(category);
                    await _unitOfWork.CompleteAsync();
                    TempData["Updated"] = "Category has been updated successfully";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(categoryVM);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var category = await _unitOfWork.Repository<Category, int>().GetByIdAsync(id);
            if (category is null)
                return BadRequest();
            try
            {
                _unitOfWork.Repository<Category, int>().Delete(category);
                await _unitOfWork.CompleteAsync();
                TempData["Deleted"] = "Category has been deleted successfully";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
