using Flagship.Application.Interfaces;
using Flagship.Core.Entities;
using Flagship.Core.Interfaces;
using Flagship.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Flagship.Application.Services {
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public UserService(IUserRepository userRepository, IConfiguration configuration) {
            _userRepository = userRepository;
            _configuration = configuration;
        }
        public async Task<User> Login(string loginName, string loginPassword)
        {
            return await _userRepository.Login(loginName, loginPassword);
        }
        public async Task<User> GetById(Int64 userId)
        {
            return await _userRepository.GetAllById(userId);
        }
        public async Task<IList<User>> GetAll()
        {
            return await _userRepository.GetAll();
        }
        public async Task<IList<User>> GetAllActive()
        {
            return await _userRepository.GetAllActive();
        }

        public async Task<JWToken> GenerateToken(long userId, string userName) {
            var dateTimeNow = DateTime.Now;
            var tokenExpireTime = dateTimeNow.AddMinutes(Convert.ToDouble(_configuration["TokenAuthentication:JWTTokenExpirationTimeInMinutes"]));
            var refreshTokenExpirationTime = dateTimeNow.AddMinutes(Convert.ToDouble(_configuration["TokenAuthentication:JWTRefreshTokenExpirationTimeInMinutes"]));

            var claims = new[]
            {
                new Claim(ClaimTypes.Sid, userId.ToString()),
                new Claim(ClaimTypes.Name, userName),
             };

            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["TokenAuthentication:SecretKey"]));

            var jwt = new JwtSecurityToken(
                issuer: _configuration["TokenAuthentication:Issuer"],
                audience: _configuration["TokenAuthentication:Audience"],
                claims: claims,
                notBefore: dateTimeNow,
                expires: tokenExpireTime,
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
            );

            return await Task.FromResult(new JWToken() {
                Token = new JwtSecurityTokenHandler().WriteToken(jwt),
                TokenExpirationTimeInMinutes = tokenExpireTime,
                RefreshTokenExpirationTimeInMinutes = refreshTokenExpirationTime
            });
        }
    }
}
