using Flagship.Application.Interfaces;
using Flagship.Core.Entities;
using Flagship.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace Flagship.Application.Services {
    public class RefreshTokenService : IRefreshTokenService {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IConfiguration _configuration;

        public RefreshTokenService(IRefreshTokenRepository refreshTokenRepository, IConfiguration configuration) {
            _refreshTokenRepository = refreshTokenRepository;
            _configuration = configuration;
        }

        public async Task<string> Issue(long userId) {
            var rawToken = GenerateRawToken();
            var expirationMinutes = Convert.ToDouble(_configuration["TokenAuthentication:JWTRefreshTokenExpirationTimeInMinutes"]);
            var now = DateTime.UtcNow;

            await _refreshTokenRepository.Insert(new RefreshToken {
                UserId = userId,
                TokenHash = Hash(rawToken),
                CreatedAtUtc = now,
                ExpiresAtUtc = now.AddMinutes(expirationMinutes)
            });

            return rawToken;
        }

        public async Task<(long UserId, string RefreshToken)?> Rotate(string refreshToken) {
            var tokenHash = Hash(refreshToken);
            var existing = await _refreshTokenRepository.GetByTokenHash(tokenHash);
            if (existing == null) return null;

            if (existing.RevokedAtUtc != null) {
                // The token was already used/rotated once before. Presenting it again means
                // either a replay attack or a stolen token — kill every session for this user.
                await _refreshTokenRepository.RevokeAllForUser(existing.UserId);
                return null;
            }

            if (existing.ExpiresAtUtc <= DateTime.UtcNow) return null;

            var newRawToken = await Issue(existing.UserId);
            await _refreshTokenRepository.Revoke(tokenHash, Hash(newRawToken));

            return (existing.UserId, newRawToken);
        }

        public async Task Revoke(string refreshToken) {
            await _refreshTokenRepository.Revoke(Hash(refreshToken), null);
        }

        public async Task RevokeAllForUser(long userId) {
            await _refreshTokenRepository.RevokeAllForUser(userId);
        }

        private static string Hash(string rawToken) =>
            Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(rawToken)));

        private static string GenerateRawToken() =>
            Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
                .Replace('+', '-').Replace('/', '_').TrimEnd('=');
    }
}
