using System.ComponentModel.DataAnnotations;

namespace Flagship.Core.ViewModels {
    public class LoginViewModel {
        [Required]
        public string LoginName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
