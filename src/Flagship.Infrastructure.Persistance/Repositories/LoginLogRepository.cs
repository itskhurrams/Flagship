using Flagship.Core.Entities;
using Flagship.Core.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Flagship.Infrastructure.Persistance.Repositories {
    public class LoginLogRepository : ILoginLogRepository {
        #region DataMembers and Properties
        private readonly IBaseRepository _baseRepository;
        #endregion

        #region Constructor
        public LoginLogRepository(IBaseRepository baseRepository) {
            _baseRepository = baseRepository;
        }
        #endregion

        #region SQL Procedures
        private const string PROC_lOGINLOG_INSERT = "usp_LoginLog_Insert";
        private const string PROC_lOGINLOG_UPDATE = "usp_LoginLog_Update";
        private const string PROC_LOGINLOG_UPDATE_BYSESSIONTOKENANDUSERID = "usp_LoginLog_Update_BySessionTokenAndUserId";
        #endregion

        #region SQL Table Columns
        private const string LOGINLOGID = "login_log_id";
        private const string USERID = "user_id";
        private const string LOGINTIME = "login_time";
        private const string LOGOUTTIME = "logout_time";
        private const string MACHINENAME = "machine_name";
        private const string IPADDRESS = "ip_address";
        private const string SERVERNAME = "server_name";
        private const string ACTIONTYPE = "action_type";
        private const string ACTIONTIME = "action_time";
        private const string SessionToken = "session_token";
        #endregion 

        #region Mapper
        #endregion

        #region Funcitons

        public async Task<bool> Insert(LoginLog loginLog) {
            using SqlConnection conn = _baseRepository.GetConnection();
            using SqlCommand cmd = _baseRepository.GetSqlCommand(conn, PROC_lOGINLOG_INSERT, true);
            cmd.Parameters.Add(_baseRepository.GetInParameter("@UserId", SqlDbType.BigInt, loginLog.UserId));
            cmd.Parameters.Add(_baseRepository.GetInParameter("@LoginTime", SqlDbType.DateTime, loginLog.LoginTime));
            cmd.Parameters.Add(_baseRepository.GetInParameter("@LogoutTime", SqlDbType.DateTime, loginLog.LogoutTime != null ? loginLog.LogoutTime : DBNull.Value));
            cmd.Parameters.Add(_baseRepository.GetInParameter("@MachineName", SqlDbType.VarChar, loginLog.MachineName != null ? loginLog.MachineName : DBNull.Value));
            cmd.Parameters.Add(_baseRepository.GetInParameter("@IPAddress", SqlDbType.VarChar, loginLog.IPAddress != null ? loginLog.IPAddress : DBNull.Value));
            cmd.Parameters.Add(_baseRepository.GetInParameter("@ServerName", SqlDbType.VarChar, loginLog.ServerName != null ? loginLog.ServerName : DBNull.Value));
            cmd.Parameters.Add(_baseRepository.GetInParameter("@ActionType", SqlDbType.SmallInt, loginLog.ActionType != null ? loginLog.ActionType : DBNull.Value));
            cmd.Parameters.Add(_baseRepository.GetInParameter("@ActionTime", SqlDbType.DateTime, loginLog.ActionTime != null ? loginLog.ActionTime : DBNull.Value));
            cmd.Parameters.Add(_baseRepository.GetInParameter("@SessionToken", SqlDbType.VarChar, loginLog.SessionToken != null ? loginLog.SessionToken : DBNull.Value));
            await cmd.ExecuteNonQueryAsync();
            return true;
        }
        public async Task<bool> Update(LoginLog loginLog) {
            using SqlConnection conn = _baseRepository.GetConnection();
            using SqlCommand cmd = _baseRepository.GetSqlCommand(conn, PROC_lOGINLOG_UPDATE, true);
            cmd.Parameters.Add(_baseRepository.GetInParameter("@LoginLogId", SqlDbType.BigInt, loginLog.LoginLogId));
            cmd.Parameters.Add(_baseRepository.GetInParameter("@UserId", SqlDbType.BigInt, loginLog.UserId));
            cmd.Parameters.Add(_baseRepository.GetInParameter("@LoginDate", SqlDbType.DateTime, loginLog.LoginTime));
            cmd.Parameters.Add(_baseRepository.GetInParameter("@LogoutDate", SqlDbType.DateTime, loginLog.LogoutTime != null ? loginLog.LogoutTime : DBNull.Value));
            cmd.Parameters.Add(_baseRepository.GetInParameter("@MachineName", SqlDbType.VarChar, loginLog.MachineName != null ? loginLog.MachineName : DBNull.Value));
            cmd.Parameters.Add(_baseRepository.GetInParameter("@IPAddress", SqlDbType.VarChar, loginLog.IPAddress != null ? loginLog.IPAddress : DBNull.Value));
            cmd.Parameters.Add(_baseRepository.GetInParameter("@ServerName", SqlDbType.VarChar, loginLog.ServerName != null ? loginLog.ServerName : DBNull.Value));
            cmd.Parameters.Add(_baseRepository.GetInParameter("@ActionType", SqlDbType.SmallInt, loginLog.ActionType != null ? loginLog.ActionType : DBNull.Value));
            cmd.Parameters.Add(_baseRepository.GetInParameter("@ActionTime", SqlDbType.DateTime, loginLog.ActionTime != null ? loginLog.ActionTime : DBNull.Value));
            cmd.Parameters.Add(_baseRepository.GetInParameter("@SessionToken", SqlDbType.VarChar, loginLog.SessionToken != null ? loginLog.SessionToken : DBNull.Value));
            await cmd.ExecuteNonQueryAsync();
            return true;
        }
        public async Task<bool> UpdateBySessionTokenAndUserId(Int64 userId, string sessionToken) {
            using SqlConnection conn = _baseRepository.GetConnection();
            using SqlCommand cmd = _baseRepository.GetSqlCommand(conn, PROC_LOGINLOG_UPDATE_BYSESSIONTOKENANDUSERID, true);
            cmd.Parameters.Add(_baseRepository.GetInParameter("@UserId", SqlDbType.BigInt, userId));
            cmd.Parameters.Add(_baseRepository.GetInParameter("@SessionToken", SqlDbType.VarChar, sessionToken));
            await cmd.ExecuteNonQueryAsync();
            return true;
        }

        #endregion
    }
}
