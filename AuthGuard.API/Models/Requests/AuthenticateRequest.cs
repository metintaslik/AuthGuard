using System.ComponentModel.DataAnnotations;

namespace AuthGuard.API.Models.Requests
{
    public class AuthenticateRequest
    {
        [Required]
        public string ClientID { get; set; } = null!;

        [Required]
        public string ClientSecret { get; set; } = null!;
    }
}