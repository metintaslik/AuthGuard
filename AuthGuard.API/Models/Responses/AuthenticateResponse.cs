namespace AuthGuard.API.Models.Responses
{
    public class AuthenticateResponse
    {
        public string Token { get; set; } = null!;
        public DateTime ExpireTime { get; set; }
    }
}