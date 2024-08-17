using Shop.DAL.Entities;

namespace Shop.PL.ViewModels
{
	public class ProductDetailsVM
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string? Description { get; set; }
		public decimal Price { get; set; }
		public string? ImageUrl { get; set; }
		public string? CategoryName { get; set; }
	}
}
