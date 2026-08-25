using System.ComponentModel.DataAnnotations;

namespace Flagship.Core.ViewModels {
    public class LoginViewModel {
        [Required]
        public required string LoginName { get; set; }

        [Required]
        public required string Password { get; set; }
    }
}
