namespace Flagship.Application.Interfaces {
    public interface IRefreshTokenService {
        Task<string> Issue(long userId);
        Task<(long UserId, string RefreshToken)?> Rotate(string refreshToken);
        Task Revoke(string refreshToken);
        Task RevokeAllForUser(long userId);
    }
}
