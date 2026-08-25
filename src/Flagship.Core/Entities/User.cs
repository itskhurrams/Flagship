namespace Flagship.Core.Entities {
    public class User : BaseEntity
    {
        public long UserId { get; set; }
        public string LoginName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
    }
}
