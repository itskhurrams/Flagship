using Flagship.Application.Interfaces;
using Flagship.Core.Entities;
using Flagship.Core.Interfaces;

namespace Flagship.Application.Services {
    public class LoginLogService : ILoginLogService
    {
        #region Properties and Data Members
        private readonly ILoginLogRepository _loginLogRepository;
        #endregion

        #region Constructor
        public LoginLogService(ILoginLogRepository loginLogRepository)
        {
            _loginLogRepository = loginLogRepository;
        }
        #endregion
 
        public async Task<bool> Insert(LoginLog loginLog)
        {
            return await _loginLogRepository.Insert(loginLog);
        }   
        public async Task<bool> Update(LoginLog loginLog)
        {
            return await _loginLogRepository.Update(loginLog);
        }
        public async Task<bool> UpdateBySessionTokenAndUserId(Int64 userId, string sessionToken)
        {
            return await _loginLogRepository.UpdateBySessionTokenAndUserId(userId , sessionToken);
        }
    }
}
