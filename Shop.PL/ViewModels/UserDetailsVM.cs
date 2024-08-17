namespace Shop.PL.ViewModels
{
    public class UserDetailsVM
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTimeOffset LockoutEnd { get; set; }
    }
}
