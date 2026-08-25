using System.ComponentModel.DataAnnotations;

namespace Flagship.Core.Models {
    public class JWToken {
        [Required]
        public string Token { get; set; } = string.Empty;
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime TokenExpirationTimeInMinutes { get; set; }
        public DateTime RefreshTokenExpirationTimeInMinutes { get; set; }
    }
}
