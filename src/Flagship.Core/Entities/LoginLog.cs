namespace Flagship.Core.Entities {
    public class LoginLog
    {
        public Int64 LoginLogId { get; set; }
        public Int64 UserId { get; set; }
        public DateTime LoginTime { get; set; }
        public DateTime? LogoutTime { get; set; }
        public string MachineName { get; set; }
        public string IPAddress { get; set; }
        public string ServerName { get; set; }
        public Int16? ActionType { get; set; }
        public DateTime? ActionTime { get; set; }
        public string SessionToken { get; set; }
    }
}
