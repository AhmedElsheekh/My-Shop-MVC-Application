namespace Shop.PL.ViewModels
{
    public class OrderHeaderAndDetailsVM
    {
        public OrderHeaderVM OrderHeaderVM { get; set; }
        public IEnumerable<OrderDetailVM> OrderDetailVM { get; set; }
    }
}
