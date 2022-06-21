using Flagship.Core.Entities;
namespace Flagship.Core.Interfaces {
    public interface IUserRepository {
        public Task<IList<User>> GetAll();
        public Task<IList<User>> GetAllActive();
        public Task<User> GetAllById(Int64 userId);
        public Task<User> Login(string loginName, string loginPassword);
    }
}
