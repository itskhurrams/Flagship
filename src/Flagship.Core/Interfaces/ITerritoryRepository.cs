using Flagship.Core.Entities;
using Flagship.Domain.Entities;

namespace Flagship.Core.Interfaces {
    public interface ITerritoryRepository
    {
        Task<IList<Province>> GetAllProvince(bool isActive = true);
        Task<IList<Division>> GetAllDivisions(bool isActive = true);

    }

}
