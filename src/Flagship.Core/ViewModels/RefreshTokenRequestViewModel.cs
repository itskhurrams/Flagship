using System.ComponentModel.DataAnnotations;

namespace Flagship.Core.ViewModels {
    public class RefreshTokenRequestViewModel {
        [Required]
        public required string RefreshToken { get; set; }
    }
}
