using Shop.DAL.Entities;

namespace Shop.PL.ViewModels
{
	public class OrderHeaderVM
	{
		public int Id { get; set; }
		public string ApplicationUserId { get; set; }
		public decimal TotalPrice { get; set; }
		public DateTime OrderDate { get; set; }
		public DateTime ShippingDate { get; set; }
		public string? OrderStatus { get; set; }
		public string? PaymentStatus { get; set; }
		public DateTime? PaymentDate { get; set; }
		public string? TrackingNumber { get; set; }
		public string? Carrier { get; set; }

		//Stripe Properties
		public string? SessionId { get; set; }
		public string? PaymentIntentId { get; set; }

		//User Address
		public string Name { get; set; }
		public string City { get; set; }
		public string Address { get; set; }
		public string PhoneNumber { get; set; }
		public string Email { get; set; }
	}
}
