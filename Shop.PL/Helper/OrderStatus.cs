namespace Shop.PL.Helper
{
    public static class OrderStatus
    {
        public static string Pending { get; set; } = "Pending";
        public static string Confirmed { get; set; } = "Confirmed";
        public static string Processing { get; set; } = "Processing";
        public static string Cancelled { get; set; } = "Cancelled";
        public static string Refunded { get; set; } = "Refunded";
        public static string Shipped { get; set; } = "Shipped";
    }
}
