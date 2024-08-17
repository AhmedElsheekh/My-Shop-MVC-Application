using System.ComponentModel.DataAnnotations;

namespace Shop.PL.ViewModels
{
    public class BasketItemVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public string? CategoryName { get; set; }

        [Range(1, 100, ErrorMessage = "Count Should Be From 1 To 100")]
        public int Count { get; set; }
        public IEnumerable<ProductDetailsVM>? RelatedItems { get; set; }
    }
}
