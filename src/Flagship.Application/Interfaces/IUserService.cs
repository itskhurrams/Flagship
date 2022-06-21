
using Flagship.Core.Entities;
using Flagship.Core.Models;

namespace Flagship.Application.Interfaces {
    public interface IUserService {
        Task<IList<User>> GetAllActive();
        Task<IList<User>> GetAll();
        Task<User> GetById(Int64 userId);
        Task<User> Login(string loginName, string loginPassword);
        Task<JWToken> GenerateToken(Int64 userId, string userName);
    }
}
