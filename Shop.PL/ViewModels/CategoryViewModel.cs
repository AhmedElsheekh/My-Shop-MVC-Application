using System.ComponentModel.DataAnnotations;

namespace Shop.PL.ViewModels
{
    public class CategoryViewModel
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "Category name is required")]
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
