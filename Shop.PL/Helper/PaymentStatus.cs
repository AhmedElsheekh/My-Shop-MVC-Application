namespace Shop.PL.Helper
{
    public static class PaymentStatus
    {
        public static string Pending { get; set; } = "Pending";
        public static string Paid { get; set; } = "Paid";
        public static string Cancelled { get; set; } = "Cancelled";
        public static string Refunded { get; set; } = "Refunded";
    }
}
