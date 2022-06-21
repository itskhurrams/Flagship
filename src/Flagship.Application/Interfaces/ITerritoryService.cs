
using Flagship.Domain.Entities;

namespace Flagship.Application.Interfaces {
    public interface ITerritoryService {
        public Task<IList<Division>> GetAllDivisions(bool isActive = true);
    }
}
