using Flagship.Core.Entities;

namespace Flagship.Core.Interfaces {
    public interface ILoginLogRepository
    {
        public Task<bool> Insert(LoginLog loginLog);

        public Task<bool> Update(LoginLog loginLog);

        public Task<bool> UpdateBySessionTokenAndUserId(Int64 userId, string sessionToken);
    }
}
