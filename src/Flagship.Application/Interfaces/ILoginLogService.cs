using Flagship.Core.Entities;

namespace Flagship.Application.Interfaces {
    public interface ILoginLogService
    {
        public Task<bool> Insert(LoginLog loginLog);
        public Task<bool> Update(LoginLog loginLog);
        public Task<bool> UpdateBySessionTokenAndUserId(Int64 userId, string sessionToken);
    }
}
