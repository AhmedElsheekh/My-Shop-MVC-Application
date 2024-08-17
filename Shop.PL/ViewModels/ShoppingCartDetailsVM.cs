using Shop.DAL.Entities;

namespace Shop.PL.ViewModels
{
    public class ShoppingCartDetailsVM
    {
        public IEnumerable<ShoppingCart> Carts { get; set; }
        public decimal Total { get; set; }
    }
}
