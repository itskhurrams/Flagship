using Flagship.Core.Entities;

namespace Flagship.Core.Interfaces {
    public interface IRefreshTokenRepository {
        Task Insert(RefreshToken refreshToken);
        Task<RefreshToken?> GetByTokenHash(string tokenHash);
        Task Revoke(string tokenHash, string? replacedByTokenHash);
        Task RevokeAllForUser(long userId);
    }
}
