using Shop.DAL.Entities;

namespace Shop.PL.ViewModels
{
    public class OrderDetailVM
    {
        public int OrderHeaderId { get; set; }
        public int ProductId { get; set; }
        public ProductDetailsVM ProductDetails { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
    }
}
