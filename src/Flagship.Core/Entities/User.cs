namespace Flagship.Core.Entities {
    public class User : BaseEntity
    {
        public long UserId { get; set; }
        public string LoginName { get; set; }
        public string LoginPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string RoleName { get; set; }   
    }
}
