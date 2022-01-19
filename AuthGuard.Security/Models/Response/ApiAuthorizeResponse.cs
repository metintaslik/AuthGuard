namespace AuthGuard.Security.Models.Response
{
    public class ApiAuthorizeResponse
    {
        public string Token { get; set; } = null!;
        public DateTime Expire { get; set; }
    }
}
