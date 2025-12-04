

namespace Store.StoreWebApi.Dtos
{
    public class AuthResponse
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime TokenExpiry { get; set; }
        public IEnumerable<string> Roles { get; set; } = new List<string>();
    }
}
