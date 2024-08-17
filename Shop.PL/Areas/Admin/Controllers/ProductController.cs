using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.BLL.Specifications;
using Shop.BLL.UnitOfWork;
using Shop.DAL.Entities;
using Shop.PL.Helper;
using Shop.PL.ViewModels;
using X.PagedList.Extensions;

namespace Shop.PL.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "Admin")]
	public class ProductController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public ProductController(IUnitOfWork unitOfWork,
			IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<IActionResult> Index(int? page, string? searchValue)
		{
			var pageNumber = page ?? 1;
			int pageSize = 5;

            IEnumerable<Shop.DAL.Entities.Product> products;
            if (!string.IsNullOrEmpty(searchValue))
            {
                var productSpec = new ProductWithCategorySpec(searchValue);
                products = await _unitOfWork.Repository<Shop.DAL.Entities.Product, int>().GetAllWithSpecAsync(productSpec);
            }
            else
            {
				var productWithCategorySpec = new ProductWithCategorySpec();
				products = await _unitOfWork.Repository<Shop.DAL.Entities.Product, int>().GetAllWithSpecAsync(productWithCategorySpec);
            }

			var mappedProducts = _mapper.Map<IEnumerable<ProductDetailsVM>>(products).ToPagedList(pageNumber, pageSize);
			return View(mappedProducts);
		}

		[HttpGet]
		public async Task<IActionResult> Create()
		{
			ProductCreateVM productCreateVM = new ProductCreateVM();
			productCreateVM.Categories = await _unitOfWork.Repository<Category, int>().GetAllAsync();
			return View(productCreateVM);
		}

		[HttpPost]
		public async Task<IActionResult> Create(ProductCreateVM productCreateVM)
		{
			if (ModelState.IsValid)
			{
				Product product = _mapper.Map<Product>(productCreateVM);
				product.ImageUrl = DocumentSettings.Upload(productCreateVM.Image, "Images");

				try
				{
					await _unitOfWork.Repository<Product, int>().AddAsync(product);
					await _unitOfWork.CompleteAsync();
					TempData["Created"] = "Product has been created successfully";
					return RedirectToAction(nameof(Index));

				}
				catch (Exception ex)
				{
					ModelState.AddModelError("", ex.Message);
				}
			}
			return View(productCreateVM);
		}

		public async Task<IActionResult> Details(int id, string viewName = "Details")
		{
			var spec = new ProductWithCategorySpec(id);

			var product = await _unitOfWork.Repository<Product, int>().GetByIdWithSpecAsync(spec);
			if (product is null)
				return NotFound($"Product is not found with id = {id}");

			var mappedProduct = _mapper.Map<ProductDetailsVM>(product);
			return View(viewName, mappedProduct);
		}

		[HttpGet]
		public async Task<IActionResult> Update(int id)
		{
            var spec = new ProductWithCategorySpec(id);

            var product = await _unitOfWork.Repository<Product, int>().GetByIdWithSpecAsync(spec);
            if (product is null)
                return NotFound($"Product is not found with id = {id}");

			ProductUpdateVM productUpdateVM = _mapper.Map<ProductUpdateVM>(product);
			productUpdateVM.Categories = await _unitOfWork.Repository<Category, int>().GetAllAsync();
            
            return View(productUpdateVM);
        }

		[HttpPost]
		public async Task<IActionResult> Update([FromRoute] int id, ProductUpdateVM productUpdateVM)
		{
			if (id != productUpdateVM.Id)
				return BadRequest();

			if (ModelState.IsValid)
			{
				if(productUpdateVM.Image is not null)
				{
                    DocumentSettings.Delete(productUpdateVM.ImageUrl, "Images");
                    productUpdateVM.ImageUrl = DocumentSettings.Upload(productUpdateVM.Image, "Images");
                }
		
				var product = _mapper.Map<Product>(productUpdateVM);

				try
				{
					_unitOfWork.Repository<Product, int>().Update(product);
					await _unitOfWork.CompleteAsync();
					TempData["Updated"] = "Product has been updated successfully";
					return RedirectToAction(nameof(Index));
				}
				catch (Exception ex)
				{
					ModelState.AddModelError("", ex.Message);
				}
			}
			return View(productUpdateVM);
		}

		public async Task<IActionResult> Delete(int id)
		{
			var product = await _unitOfWork.Repository<Product, int>().GetByIdAsync(id);
			if (product is null)
				return BadRequest();
			try
			{
				DocumentSettings.Delete(product.ImageUrl, "Images");
				_unitOfWork.Repository<Product, int>().Delete(product);
				await _unitOfWork.CompleteAsync();
				TempData["Deleted"] = "Product has been deleted successfully";
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", ex.Message);
			}

			return RedirectToAction(nameof(Index));
		}
	}
}
