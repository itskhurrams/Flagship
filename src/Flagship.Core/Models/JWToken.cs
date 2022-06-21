using System.ComponentModel.DataAnnotations;

namespace Flagship.Core.Models {
    public class JWToken {
        [Required]
        public string Token { get; set; }
        public DateTime TokenExpirationTimeInMinutes { get; set; }
        public DateTime RefreshTokenExpirationTimeInMinutes { get; set; }
    }
}
