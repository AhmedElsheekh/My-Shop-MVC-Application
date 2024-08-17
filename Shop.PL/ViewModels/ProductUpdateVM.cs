using Shop.DAL.Entities;
using System.ComponentModel.DataAnnotations;

namespace Shop.PL.ViewModels
{
    public class ProductUpdateVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Product Name is Required")]
        public string Name { get; set; }
        public string? Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
        public IFormFile? Image { get; set; }
        public string? ImageUrl { get; set; }
        public int? CategoryId { get; set; }
        
        public IEnumerable<Category>? Categories { get; set; }
    }
}
