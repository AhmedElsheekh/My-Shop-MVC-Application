using Shop.DAL.Entities;

namespace Shop.PL.ViewModels
{
    public class OrderCreateVM
    {
        public IEnumerable<ShoppingCart>? Carts { get; set; }
        public OrderHeader CustOrderHeader { get; set; }
    }
}
