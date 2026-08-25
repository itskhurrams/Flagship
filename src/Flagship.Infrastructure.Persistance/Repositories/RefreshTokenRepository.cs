using Flagship.Core.Entities;
using Flagship.Core.Interfaces;
using Flagship.Infrastructure.Common;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Flagship.Infrastructure.Persistance.Repositories {
    public class RefreshTokenRepository : IRefreshTokenRepository {
        #region DataMembers and Properties
        private readonly IBaseRepository _baseRepository;
        #endregion

        #region Constructor
        public RefreshTokenRepository(IBaseRepository baseRepository) {
            _baseRepository = baseRepository;
        }
        #endregion

        #region SQL Procedures
        private const string ProcRefreshTokenInsert = "usp_RefreshToken_Insert";
        private const string ProcRefreshTokenGetByTokenHash = "usp_RefreshToken_GetByTokenHash";
        private const string ProcRefreshTokenRevoke = "usp_RefreshToken_Revoke";
        private const string ProcRefreshTokenRevokeAllForUser = "usp_RefreshToken_RevokeAllForUser";
        #endregion

        #region SQL Table Columns
        private const string REFRESHTOKENID = "refresh_token_id";
        private const string USERID = "user_id";
        private const string TOKENHASH = "token_hash";
        private const string CREATEDATUTC = "created_at_utc";
        private const string EXPIRESATUTC = "expires_at_utc";
        private const string REVOKEDATUTC = "revoked_at_utc";
        private const string REPLACEDBYTOKENHASH = "replaced_by_token_hash";
        #endregion

        #region Functions
        public async Task Insert(RefreshToken refreshToken) {
            using SqlConnection connection = _baseRepository.GetConnection();
            using SqlCommand command = _baseRepository.GetSqlCommand(connection, ProcRefreshTokenInsert, true);
            command.Parameters.Add(_baseRepository.GetInParameter("@UserId", SqlDbType.BigInt, refreshToken.UserId));
            command.Parameters.Add(_baseRepository.GetInParameter("@TokenHash", SqlDbType.NVarChar, refreshToken.TokenHash));
            command.Parameters.Add(_baseRepository.GetInParameter("@CreatedAtUtc", SqlDbType.DateTime2, refreshToken.CreatedAtUtc));
            command.Parameters.Add(_baseRepository.GetInParameter("@ExpiresAtUtc", SqlDbType.DateTime2, refreshToken.ExpiresAtUtc));
            await command.ExecuteNonQueryAsync();
        }

        public async Task<RefreshToken?> GetByTokenHash(string tokenHash) {
            using SqlConnection connection = _baseRepository.GetConnection();
            using SqlCommand command = _baseRepository.GetSqlCommand(connection, ProcRefreshTokenGetByTokenHash, true);
            command.Parameters.Add(_baseRepository.GetInParameter("@TokenHash", SqlDbType.NVarChar, tokenHash));

            using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync()) return null;

            return new RefreshToken {
                RefreshTokenId = Conversion.ToInt64(reader[REFRESHTOKENID]),
                UserId = Conversion.ToInt64(reader[USERID]),
                TokenHash = Conversion.ToString(reader[TOKENHASH]),
                CreatedAtUtc = Conversion.ToDateTime(reader[CREATEDATUTC]),
                ExpiresAtUtc = Conversion.ToDateTime(reader[EXPIRESATUTC]),
                RevokedAtUtc = reader[REVOKEDATUTC] != DBNull.Value ? Conversion.ToDateTime(reader[REVOKEDATUTC]) : null,
                ReplacedByTokenHash = reader[REPLACEDBYTOKENHASH] != DBNull.Value ? Conversion.ToString(reader[REPLACEDBYTOKENHASH]) : null,
            };
        }

        public async Task Revoke(string tokenHash, string? replacedByTokenHash) {
            using SqlConnection connection = _baseRepository.GetConnection();
            using SqlCommand command = _baseRepository.GetSqlCommand(connection, ProcRefreshTokenRevoke, true);
            command.Parameters.Add(_baseRepository.GetInParameter("@TokenHash", SqlDbType.NVarChar, tokenHash));
            command.Parameters.Add(_baseRepository.GetInParameter("@ReplacedByTokenHash", SqlDbType.NVarChar, replacedByTokenHash != null ? replacedByTokenHash : DBNull.Value));
            await command.ExecuteNonQueryAsync();
        }

        public async Task RevokeAllForUser(long userId) {
            using SqlConnection connection = _baseRepository.GetConnection();
            using SqlCommand command = _baseRepository.GetSqlCommand(connection, ProcRefreshTokenRevokeAllForUser, true);
            command.Parameters.Add(_baseRepository.GetInParameter("@UserId", SqlDbType.BigInt, userId));
            await command.ExecuteNonQueryAsync();
        }
        #endregion
    }
}
